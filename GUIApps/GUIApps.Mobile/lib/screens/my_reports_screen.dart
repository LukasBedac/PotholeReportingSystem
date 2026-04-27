import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:provider/provider.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import '../services/auth_service.dart';
import 'edit_report_screen.dart';

class MyReportsScreen extends StatefulWidget {
  const MyReportsScreen({Key? key}) : super(key: key);

  @override
  _MyReportsScreenState createState() => _MyReportsScreenState();
}

class _MyReportsScreenState extends State<MyReportsScreen> {
  List<dynamic> _reports = [];
  bool _isLoading = true;
  String _errorMessage = '';

  @override
  void initState() {
    super.initState();
    _fetchMyReports();
  }

  Future<void> _fetchMyReports() async {
    final auth = Provider.of<AuthService>(context, listen: false);
    if (auth.accessToken == null) {
      if (mounted) {
        setState(() {
          _errorMessage = 'Not authenticated';
          _isLoading = false;
        });
      }
      return;
    }

    try {
      Map<String, dynamic> decodedToken = JwtDecoder.decode(auth.accessToken!);
      final userId = decodedToken['sub'];

      if (userId == null) {
        if (mounted) {
          setState(() {
            _errorMessage = 'User ID not found in token';
            _isLoading = false;
          });
        }
        return;
      }

      final uri = Uri.parse('http://10.0.2.2:8081/api/reports/user/$userId');
      final response = await http.get(
        uri,
        headers: {
          'Authorization': 'Bearer ${auth.accessToken}',
        },
      );

      if (response.statusCode == 200) {
        if (mounted) {
          setState(() {
            _reports = json.decode(response.body);
            _isLoading = false;
          });
        }
      } else {
        if (mounted) {
          setState(() {
            _errorMessage =
                'Failed to load reports. Status: ${response.statusCode}';
            _isLoading = false;
          });
        }
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _errorMessage = 'Error fetching reports: $e';
          _isLoading = false;
        });
      }
    }
  }

  Future<void> _deleteReport(String id) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Report'),
        content: const Text('Are you sure you want to delete this report?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Delete', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    final auth = Provider.of<AuthService>(context, listen: false);
    if (auth.accessToken == null) return;

    try {
      final uri = Uri.parse('http://10.0.2.2:8081/api/reports/$id');
      final response = await http.delete(
        uri,
        headers: {
          'Authorization': 'Bearer ${auth.accessToken}',
        },
      );

      if (response.statusCode == 204 || response.statusCode == 200) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Report deleted successfully')),
          );
        }
        _fetchMyReports();
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
                content:
                    Text('Failed to delete report: ${response.statusCode}')),
          );
        }
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error deleting report: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('My Reports'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              setState(() {
                _isLoading = true;
                _errorMessage = '';
              });
              _fetchMyReports();
            },
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_errorMessage.isNotEmpty) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Text(
            _errorMessage,
            style: const TextStyle(color: Colors.red, fontSize: 16),
            textAlign: TextAlign.center,
          ),
        ),
      );
    }

    if (_reports.isEmpty) {
      return const Center(
        child: Text('You have not submitted any reports yet.'),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(8.0),
      itemCount: _reports.length,
      itemBuilder: (context, index) {
        final report = _reports[index];
        final id = report['id'] ?? '';

        // Parse Category and Status which are returned as integers from C#
        int categoryInt = (report['category'] is int)
            ? report['category']
            : int.tryParse(report['category']?.toString() ?? '0') ?? 0;
        int statusInt = (report['status'] is int)
            ? report['status']
            : int.tryParse(report['status']?.toString() ?? '0') ?? 0;

        final category = _getCategoryString(categoryInt);
        final status = _getStatusString(statusInt);

        final reportedAt = report['reportedAt'] ?? '';
        final description = report['description'] ?? 'No description provided';
        final image = report['image'];
        String? imageUrl;
        if (image != null &&
            image['storageKey'] != null &&
            image['bucketName'] != null) {
          String bucket = image['bucketName'];
          String key = image['storageKey'];
          imageUrl = 'http://10.0.2.2:8085/api/files/view/$bucket/$key';
        }

        return Card(
          margin: const EdgeInsets.symmetric(vertical: 8.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (imageUrl != null)
                Image.network(
                  imageUrl,
                  height: 200,
                  fit: BoxFit.cover,
                  errorBuilder: (context, error, stackTrace) => const Icon(
                      Icons.broken_image,
                      size: 100,
                      color: Colors.grey),
                ),
              Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          category,
                          style: Theme.of(context).textTheme.titleLarge,
                        ),
                        Chip(
                          label: Text(
                            status,
                            style: const TextStyle(
                                color: Colors.white, fontSize: 12),
                          ),
                          backgroundColor: _getStatusColor(status),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Reported: ${_formatDate(reportedAt)}',
                      style: Theme.of(context)
                          .textTheme
                          .bodySmall
                          ?.copyWith(color: Colors.grey),
                    ),
                    const SizedBox(height: 8),
                    Text(description),
                    const SizedBox(height: 16),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.end,
                      children: [
                        TextButton.icon(
                          onPressed: () => _deleteReport(id),
                          icon: const Icon(Icons.delete, color: Colors.red),
                          label: const Text('Delete',
                              style: TextStyle(color: Colors.red)),
                        ),
                        const SizedBox(width: 8),
                        ElevatedButton.icon(
                          onPressed: () async {
                            final result = await Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    EditReportScreen(report: report),
                              ),
                            );
                            if (result == true) {
                              _fetchMyReports(); // Refresh if edited
                            }
                          },
                          icon: const Icon(Icons.edit),
                          label: const Text('Edit'),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  Color _getStatusColor(String status) {
    switch (status.toLowerCase()) {
      case 'pending':
        return Colors.orange;
      case 'in progress':
      case 'inprogress':
        return Colors.blue;
      case 'resolved':
      case 'completed':
        return Colors.green;
      case 'rejected':
      case 'duplicate':
        return Colors.red;
      default:
        return Colors.grey;
    }
  }

  String _getCategoryString(int categoryEnum) {
    switch (categoryEnum) {
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

  String _getStatusString(int statusEnum) {
    switch (statusEnum) {
      case 0:
        return 'Reported';
      case 1:
        return 'Under Review';
      case 2:
        return 'In Progress';
      case 3:
        return 'Repaired';
      case 4:
        return 'Rejected';
      case 5:
        return 'Cannot Be Repaired';
      default:
        return 'Unknown';
    }
  }

  String _formatDate(String dateString) {
    if (dateString.isEmpty) return 'Unknown';
    try {
      final date = DateTime.parse(dateString).toLocal();
      return "${date.day}.${date.month}.${date.year} ${date.hour}:${date.minute.toString().padLeft(2, '0')}";
    } catch (e) {
      return dateString;
    }
  }
}
