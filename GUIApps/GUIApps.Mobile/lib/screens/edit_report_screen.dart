import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';

class EditReportScreen extends StatefulWidget {
  final Map<String, dynamic> report;

  const EditReportScreen({Key? key, required this.report}) : super(key: key);

  @override
  _EditReportScreenState createState() => _EditReportScreenState();
}

class _EditReportScreenState extends State<EditReportScreen> {
  final _formKey = GlobalKey<FormState>();
  late String _categoryString;
  late String _description;
  bool _isSaving = false;

  late LatLng _selectedLocation;
  final MapController _mapController = MapController();

  final List<String> _categories = [
    'Pothole',
    'Cracked Road',
    'Damaged Shoulder',
    'Missing Sign',
    'Other'
  ];

  String _getCategoryString(dynamic categoryEnum) {
    int val = (categoryEnum is int)
        ? categoryEnum
        : int.tryParse(categoryEnum?.toString() ?? '0') ?? 0;
    switch (val) {
      case 0:
        return 'Pothole';
      case 1:
        return 'Cracked Road';
      case 2:
        return 'Damaged Shoulder';
      case 3:
        return 'Missing Sign';
      case 4:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  int _getCategoryInt(String categoryName) {
    switch (categoryName) {
      case 'Pothole':
        return 0;
      case 'Cracked Road':
        return 1;
      case 'Crack':
        return 1;
      case 'Damaged Shoulder':
        return 2;
      case 'Uneven Surface':
        return 2;
      case 'Missing Sign':
        return 3;
      case 'Faded Marking':
        return 3;
      case 'Other':
        return 4;
      default:
        return 0;
    }
  }

  @override
  void initState() {
    super.initState();
    _categoryString = _getCategoryString(widget.report['category']);
    _description = widget.report['description']?.toString() ?? '';

    double lat =
        double.tryParse(widget.report['latitude']?.toString() ?? '48.1486') ??
            48.1486;
    double lng = double.tryParse(widget.report['longTitude']?.toString() ??
            widget.report['longitude']?.toString() ??
            '17.1077') ??
        17.1077;
    _selectedLocation = LatLng(lat, lng);

    if (!_categories.contains(_categoryString)) {
      _categories.add(_categoryString);
    }
  }

  Future<void> _saveChanges() async {
    if (!_formKey.currentState!.validate()) return;

    _formKey.currentState!.save();

    setState(() {
      _isSaving = true;
    });

    final id = widget.report['id'];

    // Copy existing report data and override editable fields
    final updatedReport = Map<String, dynamic>.from(widget.report);
    updatedReport['category'] = _getCategoryInt(_categoryString);
    updatedReport['description'] = _description;
    updatedReport['latitude'] = _selectedLocation.latitude;
    updatedReport['longTitude'] = _selectedLocation.longitude;

    try {
      final uri = Uri.parse('http://10.0.2.2:8081/api/reports/$id');
      final response = await http.put(
        uri,
        headers: {'Content-Type': 'application/json'},
        body: json.encode(updatedReport),
      );

      if (response.statusCode == 204 || response.statusCode == 200) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Report updated successfully')),
          );
          Navigator.pop(context, true); // Return true to indicate success
        }
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
                content:
                    Text('Failed to update report: ${response.statusCode}')),
          );
        }
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error updating report: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isSaving = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Edit Report'),
      ),
      body: _isSaving
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16.0),
              child: Form(
                key: _formKey,
                child: ListView(
                  children: [
                    const Text(
                      'Category',
                      style:
                          TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    const SizedBox(height: 8),
                    DropdownButtonFormField<String>(
                      value: _categoryString,
                      decoration: const InputDecoration(
                        border: OutlineInputBorder(),
                        contentPadding:
                            EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                      ),
                      items: _categories.map((cat) {
                        return DropdownMenuItem(
                          value: cat,
                          child: Text(cat),
                        );
                      }).toList(),
                      onChanged: (value) {
                        if (value != null) {
                          setState(() {
                            _categoryString = value;
                          });
                        }
                      },
                      onSaved: (value) {
                        if (value != null) _categoryString = value;
                      },
                    ),
                    const SizedBox(height: 24),
                    const Text(
                      'Description',
                      style:
                          TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    const SizedBox(height: 8),
                    TextFormField(
                      initialValue: _description,
                      decoration: const InputDecoration(
                        border: OutlineInputBorder(),
                        hintText: 'Provide additional details...',
                      ),
                      maxLines: 5,
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Description is required';
                        }
                        return null;
                      },
                      onSaved: (value) {
                        if (value != null) _description = value.trim();
                      },
                    ),
                    const SizedBox(height: 24),
                    const Text(
                      'Location',
                      style:
                          TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    const SizedBox(height: 8),
                    Container(
                      height: 250,
                      decoration: BoxDecoration(
                        border: Border.all(color: Colors.grey),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(8),
                        child: FlutterMap(
                          mapController: _mapController,
                          options: MapOptions(
                            center: _selectedLocation,
                            zoom: 15.0,
                            onTap: (tapPosition, point) {
                              setState(() {
                                _selectedLocation = point;
                              });
                            },
                          ),
                          children: [
                            TileLayer(
                              urlTemplate:
                                  'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                              userAgentPackageName:
                                  'sk.potholereportingsystem.mobile',
                            ),
                            RichAttributionWidget(
                              attributions: [
                                TextSourceAttribution(
                                  'OpenStreetMap contributors',
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
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      'Drag the pin or click on the map to change the location.',
                      textAlign: TextAlign.center,
                      style: TextStyle(color: Colors.grey, fontSize: 12),
                    ),
                    const SizedBox(height: 32),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                      onPressed: _saveChanges,
                      child: const Text('Save Changes',
                          style: TextStyle(fontSize: 18)),
                    ),
                  ],
                ),
              ),
            ),
    );
  }
}
