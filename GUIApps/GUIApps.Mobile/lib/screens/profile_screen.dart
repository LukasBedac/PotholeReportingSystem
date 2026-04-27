import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import '../services/auth_service.dart';
import 'create_report_screen.dart';
import 'webview_login_screen.dart';
import 'my_reports_screen.dart';

class ProfileScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<AuthService>(
      builder: (context, auth, _) {
        if (!auth.isAuthenticated) {
          return _buildUnauthenticated(context, auth);
        } else {
          return _buildAuthenticated(context, auth);
        }
      },
    );
  }

  Widget _buildUnauthenticated(BuildContext context, AuthService auth) {
    return Scaffold(
      appBar: AppBar(title: const Text('My Profile')),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.account_circle, size: 80, color: Colors.grey),
              const SizedBox(height: 20),
              const Text(
                'You are not logged in.',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              const Text(
                'Log in to report potholes, track your submissions, and view your profile.',
                textAlign: TextAlign.center,
                style: TextStyle(color: Colors.black54),
              ),
              const SizedBox(height: 30),
              // ElevatedButton.icon(
              //   onPressed: () {
              //     Navigator.push(
              //       context,
              //       MaterialPageRoute(
              //           builder: (context) => CreateReportScreen()),
              //     );
              //   },
              //   icon: const Icon(Icons.assignment),
              //   label: const Text('Create Report (Guest)'),
              //   style: ElevatedButton.styleFrom(
              //     padding:
              //         const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
              //     textStyle: const TextStyle(fontSize: 18),
              //   ),
              // ),
              const SizedBox(height: 16),
              OutlinedButton.icon(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                        builder: (context) => const WebViewLoginScreen()),
                  );
                },
                icon: const Icon(Icons.login),
                label: const Text('Login with Keycloak'),
                style: OutlinedButton.styleFrom(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                  textStyle: const TextStyle(fontSize: 18),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildAuthenticated(BuildContext context, AuthService auth) {
    String fullName = "Unknown";
    String email = "No Email";
    String userId = "Unknown ID";
    List<String> roles = [];

    if (auth.accessToken != null) {
      try {
        Map<String, dynamic> decodedToken =
            JwtDecoder.decode(auth.accessToken!);
        fullName = decodedToken['name'] ?? "Unknown";
        email = decodedToken['email'] ?? "No Email";
        userId = decodedToken['sub'] ?? "Unknown ID";

        if (decodedToken['realm_access'] != null &&
            decodedToken['realm_access']['roles'] != null) {
          var allRoles =
              List<String>.from(decodedToken['realm_access']['roles']);
          roles = allRoles
              .where((r) =>
                  r.toLowerCase() == 'user' || r.toLowerCase() == 'admin')
              .map((r) => r[0].toUpperCase() + r.substring(1).toLowerCase())
              .toList();
        }
      } catch (e) {
        // Handle token decode error
      }
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Profile'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () => auth.logout(),
          )
        ],
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Card(
                elevation: 3,
                child: Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: Column(
                    children: [
                      CircleAvatar(
                        radius: 40,
                        backgroundColor: Colors.blue,
                        child: Text(
                          fullName.isNotEmpty ? fullName[0].toUpperCase() : '?',
                          style: const TextStyle(
                              fontSize: 32, color: Colors.white),
                        ),
                      ),
                      const SizedBox(height: 16),
                      Text(fullName,
                          style: Theme.of(context).textTheme.headlineSmall),
                      Text(email,
                          style: Theme.of(context)
                              .textTheme
                              .bodyMedium
                              ?.copyWith(color: Colors.grey)),
                      const Divider(height: 32),
                      SizedBox(
                        width: double.infinity,
                        child: OutlinedButton.icon(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                  builder: (context) => CreateReportScreen()),
                            );
                          },
                          icon: const Icon(Icons.assignment),
                          label: const Text('Create Report'),
                        ),
                      ),
                      const SizedBox(height: 8),
                      SizedBox(
                        width: double.infinity,
                        child: OutlinedButton.icon(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                  builder: (context) =>
                                      const MyReportsScreen()),
                            );
                          },
                          icon: const Icon(Icons.list_alt),
                          label: const Text('My Reports'),
                        ),
                      ),
                      const SizedBox(height: 8),
                      SizedBox(
                        width: double.infinity,
                        child: OutlinedButton.icon(
                          onPressed: () => auth.logout(),
                          icon: const Icon(Icons.logout, color: Colors.red),
                          label: const Text('Logout',
                              style: TextStyle(color: Colors.red)),
                          style: OutlinedButton.styleFrom(
                            side: const BorderSide(color: Colors.red),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Card(
                elevation: 3,
                child: Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Account Details',
                          style: Theme.of(context).textTheme.titleLarge),
                      const SizedBox(height: 16),
                      _DetailRow(
                          icon: Icons.person,
                          label: 'Full Name',
                          value: fullName),
                      const Divider(),
                      _DetailRow(
                          icon: Icons.email,
                          label: 'Email Address',
                          value: email),
                      const Divider(),
                      _DetailRow(
                          icon: Icons.fingerprint,
                          label: 'User ID (Sub)',
                          value: userId),
                      const Divider(),
                      const Text('Roles:',
                          style: TextStyle(
                              fontWeight: FontWeight.bold,
                              color: Colors.black54)),
                      const SizedBox(height: 8),
                      Wrap(
                        spacing: 8.0,
                        runSpacing: 4.0,
                        children: roles.isEmpty
                            ? [const Chip(label: Text('No Roles Assigned'))]
                            : roles
                                .map((r) => Chip(
                                      label: Text(r),
                                      backgroundColor: Colors.blue.shade100,
                                      side: BorderSide.none,
                                    ))
                                .toList(),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _DetailRow(
      {required this.icon, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: Colors.blue),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label,
                    style: const TextStyle(fontSize: 12, color: Colors.grey)),
                Text(value, style: const TextStyle(fontSize: 16)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
