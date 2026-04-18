import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';
import 'package:geolocator/geolocator.dart';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:provider/provider.dart';
import '../services/auth_service.dart';
import 'package:jwt_decoder/jwt_decoder.dart';

class CreateReportScreen extends StatefulWidget {
  @override
  _CreateReportScreenState createState() => _CreateReportScreenState();
}

class _CreateReportScreenState extends State<CreateReportScreen> {
  final _formKey = GlobalKey<FormState>();

  File? _image;
  final ImagePicker _picker = ImagePicker();

  LatLng _selectedLocation = LatLng(48.1486, 17.1077); // Default to Bratislava
  final MapController _mapController = MapController();

  final TextEditingController _latController = TextEditingController();
  final TextEditingController _lngController = TextEditingController();
  final TextEditingController _cityController = TextEditingController();
  final TextEditingController _okresController = TextEditingController();
  final TextEditingController _obecController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();

  String _selectedCategory = 'Pothole';
  final List<String> _categories = [
    'Pothole',
    'CrackedRoad',
    'DamagedShoulder',
    'MissingSign',
    'Other',
  ];

  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    _determinePosition();
    _updateControllers();
  }

  Future<void> _determinePosition() async {
    bool serviceEnabled;
    LocationPermission permission;

    serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) return;

    permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) return;
    }

    if (permission == LocationPermission.deniedForever) return;

    Position position = await Geolocator.getCurrentPosition();
    setState(() {
      _selectedLocation = LatLng(position.latitude, position.longitude);
      _mapController.move(_selectedLocation, 15.0);
      _updateControllers();
    });
  }

  void _updateControllers() {
    _latController.text = _selectedLocation.latitude.toString();
    _lngController.text = _selectedLocation.longitude.toString();
    _reverseGeocode(_selectedLocation.latitude, _selectedLocation.longitude);
  }

  Future<void> _reverseGeocode(double lat, double lng) async {
    try {
      final url =
          'https://nominatim.openstreetmap.org/reverse?format=json&lat=$lat&lon=$lng&zoom=10&addressdetails=1';
      final response = await http.get(
        Uri.parse(url),
        headers: {'User-Agent': 'sk.potholereportingsystem.mobile/1.0'},
      );

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        if (data['address'] != null) {
          final address = data['address'];
          setState(() {
            _cityController.text = address['city'] ??
                address['town'] ??
                address['village'] ??
                address['municipality'] ??
                '';
            String county = address['county'] ?? '';
            _okresController.text = county
                .replaceAll('okres ', '')
                .replaceAll('District', '')
                .trim();
            String state = address['state'] ?? address['region'] ?? '';
            _obecController.text =
                state.replaceAll('kraj', '').replaceAll('Region', '').trim();
          });
        }
      }
    } catch (e) {
      debugPrint('Reverse geocoding failed: $e');
    }
  }

  Future<void> _geocodeCity(String city) async {
    if (city.isEmpty) return;
    try {
      final url =
          'https://nominatim.openstreetmap.org/search?format=json&city=${Uri.encodeComponent(city)}&countrycodes=sk';
      final response = await http.get(
        Uri.parse(url),
        headers: {'User-Agent': 'sk.potholereportingsystem.mobile/1.0'},
      );

      if (response.statusCode == 200) {
        final data = json.decode(response.body) as List;
        if (data.isNotEmpty) {
          final firstResult = data[0];
          final lat = double.tryParse(firstResult['lat'].toString());
          final lon = double.tryParse(firstResult['lon'].toString());
          if (lat != null && lon != null) {
            setState(() {
              _selectedLocation = LatLng(lat, lon);
              _mapController.move(_selectedLocation, 14.0);
              _latController.text = lat.toString();
              _lngController.text = lon.toString();
            });
            // We populate Okres/Obec too
            _reverseGeocode(lat, lon);
          }
        }
      }
    } catch (e) {
      debugPrint('Forward geocoding failed: $e');
    }
  }

  Future<void> _pickImage(ImageSource source) async {
    final XFile? photo = await _picker.pickImage(source: source);
    if (photo != null) {
      setState(() {
        _image = File(photo.path);
      });
    }
  }

  Future<void> _submitReport() async {
    if (!_formKey.currentState!.validate()) return;
    if (_image == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select or take a photo')),
      );
      return;
    }

    final authService = Provider.of<AuthService>(context, listen: false);
    if (!authService.isAuthenticated || authService.accessToken == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
            content: Text('You must be logged in to report a pothole')),
      );
      return;
    }

    setState(() => _isSubmitting = true);

    try {
      // 1. Decode token to get user ID
      Map<String, dynamic> decodedToken =
          JwtDecoder.decode(authService.accessToken!);
      String userId = decodedToken['sub'] ?? '';

      // 2. Upload file to FileService
      var request = http.MultipartRequest(
          'POST', Uri.parse('http://10.0.2.2:8085/api/files/upload'));
      request.files
          .add(await http.MultipartFile.fromPath('file', _image!.path));

      var fileResponse = await request.send();
      if (fileResponse.statusCode != 200) {
        throw Exception(
            'Failed to upload image (Status: ${fileResponse.statusCode})');
      }

      var responseData = await fileResponse.stream.bytesToString();
      var uploadResult = json.decode(responseData);

      // 3. Post to ReportService
      int categoryIndex = _categories.indexOf(_selectedCategory);
      if (categoryIndex == -1) categoryIndex = 0; // fallback to Pothole

      Map<String, dynamic> reportData = {
        'userID': userId,
        'latitude': _selectedLocation.latitude,
        'longTitude': _selectedLocation.longitude,
        'description': _descriptionController.text.trim(),
        'city': _cityController.text.trim(),
        'district': _okresController.text.trim(),
        'region': _obecController.text.trim(),
        'category': categoryIndex, // Backend enum PotholeCategory
        'status': 0, // Backend enum ReportStatus.Reported
        'image': {
          'storageKey': uploadResult['storageKey'],
          'bucketName': uploadResult['bucketName'],
          'originalName': uploadResult['originalName'],
          'contentType': uploadResult['contentType'],
          'sizeBytes': uploadResult['size'],
        }
      };

      var reportResponse = await http.post(
        Uri.parse('http://10.0.2.2:8081/api/reports'),
        headers: {
          'Content-Type': 'application/json',
          // Optionally add Authorization header if your internal endpoints require them:
          // 'Authorization': 'Bearer ${authService.accessToken}'
        },
        body: json.encode(reportData),
      );

      if (reportResponse.statusCode == 200 ||
          reportResponse.statusCode == 201) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Report submitted successfully!')),
        );
        _reset();
      } else {
        throw Exception(
            'Failed to submit report. Server returned: ${reportResponse.statusCode}');
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to submit report: $e')),
      );
    } finally {
      setState(() => _isSubmitting = false);
    }
  }

  void _reset() {
    setState(() {
      _image = null;
      _cityController.clear();
      _okresController.clear();
      _obecController.clear();
      _descriptionController.clear();
      _selectedCategory = _categories.first;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create Report')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Text(
                'Photo',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              if (_image != null)
                Image.file(_image!, height: 250, fit: BoxFit.cover)
              else
                Container(
                  height: 200,
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Center(
                    child: Text('Drag & drop photo here or click to select'),
                  ),
                ),
              const SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  ElevatedButton.icon(
                    onPressed: () => _pickImage(ImageSource.camera),
                    icon: const Icon(Icons.camera_alt),
                    label: const Text('Camera'),
                  ),
                  ElevatedButton.icon(
                    onPressed: () => _pickImage(ImageSource.gallery),
                    icon: const Icon(Icons.photo_library),
                    label: const Text('Gallery'),
                  ),
                ],
              ),
              const Divider(height: 32),
              const Text(
                'Map Location',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              Container(
                height: 250,
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey),
                ),
                child: FlutterMap(
                  mapController: _mapController,
                  options: MapOptions(
                    center: _selectedLocation,
                    zoom: 13.0,
                    onTap: (tapPosition, point) {
                      setState(() {
                        _selectedLocation = point;
                        _updateControllers();
                      });
                    },
                  ),
                  children: [
                    TileLayer(
                      urlTemplate:
                          'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                      userAgentPackageName: 'sk.potholereportingsystem.mobile',
                    ),
                    RichAttributionWidget(
                      attributions: [
                        TextSourceAttribution(
                          'OpenStreetMap contributors',
                          onTap: () => print('OpenStreetMap contributors'),
                        ),
                      ],
                    ),
                    MarkerLayer(
                      markers: [
                        Marker(
                          point: _selectedLocation,
                          child: const Icon(
                            Icons.location_pin,
                            color: Colors.red,
                            size: 40,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 8),
              const Text(
                'Drag the pin or click on the map to set the exact location.',
                textAlign: TextAlign.center,
                style: TextStyle(color: Colors.grey),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _latController,
                      decoration: const InputDecoration(
                        labelText: 'Latitude',
                        border: OutlineInputBorder(),
                      ),
                      readOnly: true,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _lngController,
                      decoration: const InputDecoration(
                        labelText: 'Longitude',
                        border: OutlineInputBorder(),
                      ),
                      readOnly: true,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _cityController,
                decoration: InputDecoration(
                  labelText: 'City *',
                  border: const OutlineInputBorder(),
                  suffixIcon: IconButton(
                    icon: const Icon(Icons.search),
                    onPressed: () => _geocodeCity(_cityController.text),
                  ),
                ),
                textInputAction: TextInputAction.search,
                onFieldSubmitted: (val) => _geocodeCity(val),
                validator: (val) => val!.isEmpty ? 'City is required' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _okresController,
                decoration: const InputDecoration(
                  labelText: 'Okres',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _obecController,
                decoration: const InputDecoration(
                  labelText: 'Obec (Region)',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                value: _selectedCategory,
                items: _categories
                    .map((c) => DropdownMenuItem(value: c, child: Text(c)))
                    .toList(),
                onChanged: (val) {
                  setState(() => _selectedCategory = val!);
                },
                decoration: const InputDecoration(
                  labelText: 'Category *',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              const TextField(
                enabled: false,
                decoration: InputDecoration(
                  labelText: 'Status (Reported automatically)',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descriptionController,
                decoration: const InputDecoration(
                  labelText: 'Description',
                  border: OutlineInputBorder(),
                ),
                maxLines: 4,
              ),
              const SizedBox(height: 32),
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  OutlinedButton(onPressed: _reset, child: const Text('Reset')),
                  const SizedBox(width: 16),
                  ElevatedButton(
                    onPressed: _isSubmitting ? null : _submitReport,
                    child: _isSubmitting
                        ? const CircularProgressIndicator()
                        : const Text('Submit'),
                  ),
                ],
              ),
              const SizedBox(height: 32),
            ],
          ),
        ),
      ),
    );
  }
}
