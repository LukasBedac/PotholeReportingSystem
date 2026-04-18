import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'screens/profile_screen.dart';
import 'services/auth_service.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final authService = AuthService();
  await authService.init();

  runApp(
    MultiProvider(
      providers: [ChangeNotifierProvider.value(value: authService)],
      child: MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Pothole Reporting',
      theme: ThemeData(primarySwatch: Colors.blue, useMaterial3: true),
      home: ProfileScreen(),
    );
  }
}
