import 'dart:convert';
import 'dart:math';
import 'package:crypto/crypto.dart';
import 'package:http/http.dart' as http;
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class AuthService extends ChangeNotifier {
  bool _isAuthenticated = false;
  String? _accessToken;
  String? _idToken;
  String? _codeVerifier;

  bool get isAuthenticated => _isAuthenticated;
  String? get accessToken => _accessToken;

  final String _clientId = 'pothole-mobile-client';
  final String redirectUrl = 'com.example.guiappsmobile://login-callback';
  final String _issuer = 'http://10.0.2.2:8080/realms/PotholeReportSystem';
  final String _authEndpoint =
      'http://10.0.2.2:8080/realms/PotholeReportSystem/protocol/openid-connect/auth';
  final String _tokenEndpoint =
      'http://10.0.2.2:8080/realms/PotholeReportSystem/protocol/openid-connect/token';
  final String _endSessionEndpoint =
      'http://10.0.2.2:8080/realms/PotholeReportSystem/protocol/openid-connect/logout';

  Future<void> init() async {
    final prefs = await SharedPreferences.getInstance();
    _accessToken = prefs.getString('access_token');
    _idToken = prefs.getString('id_token');

    if (_accessToken != null && _idToken != null) {
      _isAuthenticated = true;
      notifyListeners();
    }
  }

  String _generateRandomString(int length) {
    const chars =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~';
    final random = Random.secure();
    return List.generate(length, (index) => chars[random.nextInt(chars.length)])
        .join();
  }

  String _generateCodeChallenge(String verifier) {
    final bytes = utf8.encode(verifier);
    final digest = sha256.convert(bytes);
    return base64UrlEncode(digest.bytes).replaceAll('=', '');
  }

  // Generate URL to load in WebView
  String getAuthUrl() {
    _codeVerifier = _generateRandomString(64);
    final codeChallenge = _generateCodeChallenge(_codeVerifier!);

    return '$_authEndpoint?client_id=$_clientId&response_type=code'
        '&redirect_uri=${Uri.encodeComponent(redirectUrl)}'
        '&scope=openid%20profile%20email'
        '&code_challenge=$codeChallenge'
        '&code_challenge_method=S256';
  }

  // Exchange the intercepted code for tokens
  Future<void> exchangeCodeForToken(String code) async {
    try {
      final response = await http.post(
        Uri.parse(_tokenEndpoint),
        headers: {'Content-Type': 'application/x-www-form-urlencoded'},
        body: {
          'client_id': _clientId,
          'grant_type': 'authorization_code',
          'code': code,
          'redirect_uri': redirectUrl,
          if (_codeVerifier != null) 'code_verifier': _codeVerifier!,
        },
      );

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        _accessToken = data['access_token'];
        _idToken = data['id_token'];
        _isAuthenticated = true;

        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('access_token', _accessToken!);
        if (_idToken != null) {
          await prefs.setString('id_token', _idToken!);
        }

        notifyListeners();
      } else {
        throw Exception(
            'Failed to exchange code: ${response.statusCode} - ${response.body}');
      }
    } catch (e) {
      _isAuthenticated = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> logout() async {
    _isAuthenticated = false;
    _accessToken = null;

    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('access_token');

    if (_idToken != null) {
      try {
        await http.post(
          Uri.parse(_endSessionEndpoint),
          headers: {'Content-Type': 'application/x-www-form-urlencoded'},
          body: {
            'client_id': _clientId,
            'id_token_hint': _idToken,
          },
        );
      } catch (e) {
        debugPrint('Logout request failed: $e');
      }
      await prefs.remove('id_token');
      _idToken = null;
    }
    notifyListeners();
  }
}
