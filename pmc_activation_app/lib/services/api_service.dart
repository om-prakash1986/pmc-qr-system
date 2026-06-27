import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../models/staff_user.dart';
import '../models/qr_card.dart';
import '../models/property.dart';
import '../models/property_detail_row.dart';
import '../models/tax_history_row.dart';
import 'package:geolocator/geolocator.dart';
class ApiService extends ChangeNotifier {
  static const String keyBaseUrl = 'pmc_base_url';
  static const String keyUserSession = 'pmc_user_session';
  static String get defaultBaseUrl => kIsWeb ? 'http://localhost/backend_api' : 'http://187.127.178.111/backend_api';
  String _baseUrl = defaultBaseUrl; // server root, no /api prefix
  StaffUser? _currentUser;
  bool _isLoading = false;

  String get baseUrl => _baseUrl;
  StaffUser? get currentUser => _currentUser;
  bool get isLoading => _isLoading;
  bool get isAuthenticated => _currentUser != null;

  // Local Statistics and Logs (persisted)
  List<Map<String, dynamic>> _recentActivations = [];
  List<Map<String, dynamic>> get recentActivations => _recentActivations;
  int get activationCount => _recentActivations.length;



  ApiService() {
    _loadSettings();
  }

  Future<void> _loadSettings() async {
    final prefs = await SharedPreferences.getInstance();
    _baseUrl = prefs.getString(keyBaseUrl) ?? defaultBaseUrl;
    // Force local testing URL even if an old URL is cached
    if (_baseUrl.contains('arvixe.com') || _baseUrl.contains('localhost') || _baseUrl.contains(':55500')) {
      _baseUrl = defaultBaseUrl;
      await prefs.setString(keyBaseUrl, defaultBaseUrl); // Overwrite the cached value
    }
    // Load active session
    final sessionJson = prefs.getString(keyUserSession);
    if (sessionJson != null) {
      try {
        _currentUser = StaffUser.fromJson(jsonDecode(sessionJson));
      } catch (_) {
        prefs.remove(keyUserSession);
      }
    }

    notifyListeners();
  }



  Future<void> _saveActivationsLog() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(
      'pmc_activations_log',
      jsonEncode(_recentActivations),
    );
  }

  // ── HELPERS ───────────────────────────────────────────────────────────
  Map<String, String> get _headers {
    final Map<String, String> h = {'Content-Type': 'application/json'};
    // Override Host header for mobile testing to bypass IIS Express 400 Bad Request 
    if (!kIsWeb && _baseUrl.contains(':55500')) {
      h['Host'] = 'localhost:55500';
    }
    return h;
  }

  /// Safely decode JSON — returns null instead of throwing FormatException
  Map<String, dynamic>? _safeJsonDecode(String body) {
    try {
      return jsonDecode(body) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }

  Future<void> setBaseUrl(String url) async {
    _baseUrl = url.endsWith('/') ? url.substring(0, url.length - 1) : url;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(keyBaseUrl, _baseUrl);
    notifyListeners();
  }

  // ── API: STAFF LOGIN ──────────────────────────────────────────────────

  Future<bool> login(String username, String password) async {
    _isLoading = true;
    notifyListeners();

    try {
        // Real HTTP Call — send as JSON to match LoginHandler.ashx expectation
        final url = Uri.parse('$_baseUrl/LoginHandler.ashx');
        final payload = {'username': username.trim(), 'password': password};
        print('🔍 Staff login → $url');
        print('🔍 Payload: $payload');

        final response = await http
            .post(
              url,
              headers: _headers,
              body: jsonEncode(payload),
            )
            .timeout(const Duration(seconds: 60));

        print('🔍 Status: ${response.statusCode}');
        print('🔍 Body: ${response.body}');

        final data = _safeJsonDecode(response.body);
        if (response.statusCode == 200 && data != null && data['success'] == true) {
          _currentUser = StaffUser.fromJson(data['data']);
          final prefs = await SharedPreferences.getInstance();
          await prefs.setString(
            keyUserSession,
            jsonEncode(_currentUser!.toJson()),
          );
          _isLoading = false;
          notifyListeners();
          return true;
        } else {
          _isLoading = false;
          notifyListeners();
          final errorMsg = data?['message'] ?? 'Login failed. Code: ${response.statusCode}, Body: ${response.body.length > 100 ? response.body.substring(0, 100) : response.body}';
          throw Exception(errorMsg);
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: TAX COLLECTOR LOGIN ────────────────────────────────────────────────
  Future<bool> taxCollectorLogin(String username, String password) async {
    _isLoading = true;
    notifyListeners();
    try {
      // Use GET request as the API expects query parameters
      final uri = Uri.parse('$_baseUrl/CitizenPortal.ashx').replace(
        queryParameters: {
          'action': 'tax_login',
          'username': username,
          'password': password,
        },
      );
      final response = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 60));
      // Debug logs
      print('🔍 Tax login request to: $uri');
      print('🔍 Status code: ${response.statusCode}');
      print('🔍 Response body: ${response.body}');

      final data = jsonDecode(response.body);
      if (response.statusCode == 200 && data['success'] == true) {
        // Optionally store tax collector session if needed
        _isLoading = false;
        notifyListeners();
        return true;
      } else {
        _isLoading = false;
        notifyListeners();
        throw Exception(data['message'] ?? 'Login failed');
      }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      print('❗️ Tax login exception: $e');
      rethrow;
    }
  }

  // Helper to extract QR ID and PID from various payload formats
  static Map<String, String> extractPayload(String scannedValue) {
    Map<String, String> result = {};

    // 1. Try parsing as JSON
    try {
      final decoded = jsonDecode(scannedValue);
      if (decoded is Map) {
        if (decoded.containsKey('qr_id')) result['qrId'] = decoded['qr_id'].toString();
        else if (decoded.containsKey('qrId')) result['qrId'] = decoded['qrId'].toString();
        
        if (decoded.containsKey('pid')) result['pid'] = decoded['pid'].toString();
        else if (decoded.containsKey('property_id')) result['pid'] = decoded['property_id'].toString();
        
        if (result.isNotEmpty) return result;
      }
    } catch (_) {}

    // 2. Try parsing as URL
    if (scannedValue.startsWith('http://') || scannedValue.startsWith('https://')) {
      try {
        final uri = Uri.parse(scannedValue);
        
        if (uri.queryParameters.containsKey('qrId')) result['qrId'] = uri.queryParameters['qrId']!;
        else if (uri.queryParameters.containsKey('token')) result['qrId'] = uri.queryParameters['token']!;
        else if (uri.queryParameters.containsKey('id')) result['qrId'] = uri.queryParameters['id']!;
        else if (uri.pathSegments.isNotEmpty) result['qrId'] = uri.pathSegments.last;

        if (uri.queryParameters.containsKey('pid')) result['pid'] = uri.queryParameters['pid']!;
        else if (uri.queryParameters.containsKey('propertyId')) result['pid'] = uri.queryParameters['propertyId']!;

        return result;
      } catch (_) {}
    }

    // 3. Try parsing as comma-separated key-value pairs
    if (scannedValue.contains(':')) {
        final parts = scannedValue.split(',');
        for (var part in parts) {
            final kv = part.split(':');
            if (kv.length == 2) {
                final key = kv[0].trim().toLowerCase();
                final val = kv[1].trim();
                if (key == 'qr_id' || key == 'qrid' || key == 'token' || key == 'id') result['qrId'] = val;
                if (key == 'pid' || key == 'property_id' || key == 'propertyid') result['pid'] = val;
            }
        }
        if (result.isNotEmpty) return result;
    }

    // 4. Fallback to basic inference
    final isNumeric = double.tryParse(scannedValue) != null;
    if (isNumeric) {
      result['pid'] = scannedValue;
    } else {
      result['qrId'] = scannedValue;
    }

    return result;
  }

  // ── API: LOOKUP QR CARD ────────────────────────────────────────────────
  Future<QrCard> lookupCard(String scannedValue) async {
    _isLoading = true;
    notifyListeners();

    final extractedValue = extractPayload(scannedValue)['qrId'] ?? scannedValue;

    try {
        // Real HTTP Call
        final response = await http
            .get(
              Uri.parse(
                '$_baseUrl/CardLookupHandler.ashx?value=${Uri.encodeComponent(extractedValue)}',
              ),
              headers: _headers,
            )
            .timeout(const Duration(seconds: 60));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          return QrCard.fromJson(data['data']);
        } else {
          throw Exception(data['message'] ?? 'Card not found');
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: LOOKUP PROPERTY ──────────────────────────────────────────────
  Future<Property> lookupProperty(String pid) async {
    _isLoading = true;
    notifyListeners();

    try {
        // Real HTTP Call
        final response = await http
            .get(
              Uri.parse(
                '$_baseUrl/PropertyLookupHandler.ashx?pid=${Uri.encodeComponent(pid)}',
              ),
              headers: _headers,
            )
            .timeout(const Duration(seconds: 60));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          return Property.fromJson(data['data']);
        } else {
          throw Exception(data['message'] ?? 'Property not found');
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: LOOKUP EXTENDED PROPERTY DETAILS ──────────────────────────────
  Future<List<PropertyDetailRow>> lookupTaxCollectorPropertyDetails(String searchQuery) async {
    _isLoading = true;
    notifyListeners();

    try {

        final response = await http
            .get(
              Uri.parse('$_baseUrl/TaxCollectorPropertyDetailsHandler.ashx?search=${Uri.encodeComponent(searchQuery)}'),
              headers: _headers,
            )
            .timeout(const Duration(seconds: 60));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          final List<dynamic> list = data['data'];
          return list.map((item) => PropertyDetailRow.fromJson(item)).toList();
        } else {
          throw Exception(data['message'] ?? 'Extended property details not found');
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: TAX HISTORY ──────────────────────────────────────────────────
  Future<List<TaxHistoryRow>> fetchTaxHistory(String pid) async {
    _isLoading = true;
    notifyListeners();

    try {

        final timestamp = DateTime.now().millisecondsSinceEpoch;
        final response = await http
            .get(
              Uri.parse('$_baseUrl/TaxHistoryHandler.ashx?pid=${Uri.encodeComponent(pid)}&_t=$timestamp'),
              headers: _headers,
            )
            .timeout(const Duration(seconds: 60));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          final List<dynamic> list = data['data'];
          return list.map((item) => TaxHistoryRow.fromJson(item)).toList();
        } else {
          throw Exception(data['message'] ?? 'Tax history not found');
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: ACTIVATE CARD ────────────────────────────────────────────────
  Future<void> activateCard(String qrId, String pid, String qrToken) async {
    _isLoading = true;
    notifyListeners();

    try {
      final staffId = _currentUser?.loginId ?? 'UNKNOWN';



        // Provide default fallback dummy coordinates (Patna) for desktop simulators 
        // where native location plugins might fail or permissions might be denied.
        String geoLat = '25.6185';
        String geoLong = '85.1276';
        try {
          bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
          if (serviceEnabled) {
            LocationPermission permission = await Geolocator.checkPermission();
            if (permission == LocationPermission.denied) {
              permission = await Geolocator.requestPermission();
            }
            if (permission == LocationPermission.whileInUse || permission == LocationPermission.always) {
              Position position = await Geolocator.getCurrentPosition(
                desiredAccuracy: LocationAccuracy.high,
                timeLimit: const Duration(seconds: 15),
              );
              geoLat = position.latitude.toString();
              geoLong = position.longitude.toString();
            }
          }
        } catch (e) {
          print('Could not fetch location: $e');
        }

        // Real HTTP Call
        final response = await http
            .post(
              Uri.parse('$_baseUrl/ActivateCardHandler.ashx'),
              headers: _headers,
              body: jsonEncode({
                'qrid': qrId,
                'pid': pid,
                'staff_id': staffId,
                'device_id': 'flutter_device_agent',
                'geo_lat': geoLat,
                'geo_long': geoLong,
              }),
            )
            .timeout(const Duration(seconds: 60));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          // Log locally for history
          final newLog = {
            'qrId': qrId,
            'pid': pid,
            'owner': 'Linked successfully',
            'activatedAt': DateTime.now().toString().substring(0, 19),
          };
          _recentActivations.insert(0, newLog);
          await _saveActivationsLog();
        } else {
          throw Exception(data['message'] ?? 'Activation failed');
        }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── LOGOUT ────────────────────────────────────────────────────────────
  Future<void> logout() async {
    _currentUser = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(keyUserSession);
    notifyListeners();
  }

  // Clear activation history
  Future<void> clearHistory() async {
    _recentActivations.clear();
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('pmc_activations_log');
    notifyListeners();
  }

  // ── PROCESS PAYMENT ──────────────────────────────────────────────────
  Future<void> processPayment(
      String pid, double amount, String paymentId, {String? ownerName, String? mobileNo}) async {
    try {

      final response = await http.post(
        Uri.parse('$_baseUrl/ProcessPaymentHandler.ashx'),
        headers: _headers,
        body: jsonEncode({
          'pid': pid,
          'amount': amount,
          'paymentId': paymentId,
          'ownerName': ownerName ?? '',
          'mobileNo': mobileNo ?? '',
        }),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        if (data['success'] != true) {
          throw Exception(data['message'] ?? 'Payment processing failed');
        }
      } else {
        throw Exception('Server error: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Failed to process payment: $e');
    }
  }

  // ── API: CITIZEN PORTAL — SEND OTP ────────────────────────────────────
  /// Calls CitizenPortal.ashx?action=send_otp on the real backend.
  /// The backend validates the mobile against DB, generates an OTP,
  /// stores it in session, and dispatches it via SMS gateway.
  Future<void> sendOtp({required String token, required String mobile}) async {
    _isLoading = true;
    notifyListeners();

    try {
      final url = Uri.parse('$_baseUrl/CitizenPortal.ashx?action=send_otp');
      print('🔍 sendOtp → $url  token=$token  mobile=$mobile');

      final response = await http
          .post(
            url,
            headers: _headers,
            body: jsonEncode({'token': token, 'mobile': mobile}),
          )
          .timeout(const Duration(seconds: 60));

      print('🔍 sendOtp status: ${response.statusCode}');
      print('🔍 sendOtp body:   ${response.body}');

      final data = _safeJsonDecode(response.body);
      _isLoading = false;
      notifyListeners();

      if (response.statusCode == 200 && data != null && data['success'] == true) {
        return; // OTP sent successfully — user will receive it via SMS
      } else {
        throw Exception(data?['message'] ?? 'Failed to send OTP. Please try again.');
      }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }

  // ── API: CITIZEN PORTAL — VERIFY OTP ──────────────────────────────────
  /// Calls CitizenPortal.ashx?action=verify_otp.
  /// The C# handler verifies the session OTP, then calls
  /// PMC.demandbysms.getDemandForMutation(propertyId) and returns property data.
  Future<Property> verifyOtp({required String token, required String otp}) async {
    _isLoading = true;
    notifyListeners();

    try {
      final url = Uri.parse('$_baseUrl/CitizenPortal.ashx?action=verify_otp');
      print('🔍 verifyOtp → $url  token=$token  otp=$otp');

      final response = await http
          .post(
            url,
            headers: _headers,
            body: jsonEncode({'token': token, 'otp': otp}),
          )
          .timeout(const Duration(seconds: 60));

      print('🔍 verifyOtp status: ${response.statusCode}');
      print('🔍 verifyOtp body:   ${response.body}');

      final data = _safeJsonDecode(response.body);
      _isLoading = false;
      notifyListeners();

      if (response.statusCode == 200 && data != null && data['success'] == true) {
        // data['data'] contains property details + totalDues from getDemandForMutation
        return Property.fromJson(Map<String, dynamic>.from(data['data']));
      } else {
        throw Exception(data?['message'] ?? 'OTP verification failed. Please try again.');
      }
    } catch (e) {
      _isLoading = false;
      notifyListeners();
      rethrow;
    }
  }
}
