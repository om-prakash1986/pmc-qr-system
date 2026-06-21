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
  static const String keyIsSimulated = 'pmc_is_simulated';
  static const String keyUserSession = 'pmc_user_session';
  static const String keyLocalQrs = 'pmc_local_qrs';
  static const String defaultBaseUrl = 'http://localhost:55500';

  bool _isSimulated = false; // Real mode enabled
  String _baseUrl = defaultBaseUrl; // server root, no /api prefix
  StaffUser? _currentUser;
  bool _isLoading = false;

  bool get isSimulated => _isSimulated;
  String get baseUrl => _baseUrl;
  StaffUser? get currentUser => _currentUser;
  bool get isLoading => _isLoading;
  bool get isAuthenticated => _currentUser != null;

  // Local Statistics and Logs (persisted)
  List<Map<String, dynamic>> _recentActivations = [];
  List<Map<String, dynamic>> get recentActivations => _recentActivations;
  int get activationCount => _recentActivations.length;

  // In-Memory Database for Simulation Mode
  final Map<String, Map<String, dynamic>> _simulatedQrs = {
    'CEF733F3-7416-4FFA-87F4-2D4361DA8791': {
      'qrId': 'PMC/IND/DQR/00457417',
      'qrToken': 'CEF733F3-7416-4FFA-87F4-2D4361DA8791',
      'status': 'UNASSIGNED',
      'propertyId': '',
      'createdDate': '2026-06-17 09:34:44',
      'activatedDate': '',
      'activatedBy': '',
      'qrUrl':
          'https://pmc.bihar.gov.in/qr/CEF733F3-7416-4FFA-87F4-2D4361DA8791',
    },
    '4F23CAC1-2D0E-406A-A0DB-54300D5C46EB': {
      'qrId': 'PMC/IND/DQR/00457418',
      'qrToken': '4F23CAC1-2D0E-406A-A0DB-54300D5C46EB',
      'status': 'UNASSIGNED',
      'propertyId': '',
      'createdDate': '2026-06-17 09:34:44',
      'activatedDate': '',
      'activatedBy': '',
      'qrUrl':
          'https://pmc.bihar.gov.in/qr/4F23CAC1-2D0E-406A-A0DB-54300D5C46EB',
    },
    'BF388527-E492-4176-AC8E-0042633BADB3': {
      'qrId': 'PMC/IND/DQR/00457419',
      'qrToken': 'BF388527-E492-4176-AC8E-0042633BADB3',
      'status': 'UNASSIGNED',
      'propertyId': '',
      'createdDate': '2026-06-17 09:34:44',
      'activatedDate': '',
      'activatedBy': '',
      'qrUrl':
          'https://pmc.bihar.gov.in/qr/BF388527-E492-4176-AC8E-0042633BADB3',
    },
    // Already Activated to test security error
    '5A9F9999-BB99-99AA-AA99-AA9999999999': {
      'qrId': 'PMC/IND/DQR/00004567',
      'qrToken': '5A9F9999-BB99-99AA-AA99-AA9999999999',
      'status': 'ACTIVATED',
      'propertyId': '1409113',
      'createdDate': '2026-06-01 10:00:00',
      'activatedDate': '2026-06-17 12:00:00',
      'activatedBy': 'EMP101',
      'qrUrl':
          'https://pmc.bihar.gov.in/qr/5A9F9999-BB99-99AA-AA99-AA9999999999',
    },
  };

  final Map<String, Map<String, dynamic>> _simulatedProperties = {
    '1409113': {
      'pid': '1409113',
      'ownerName': 'SRI KUMUD VISHAL',
      'guardianName': 'SRI BALMIKI PRASAD SINGH',
      'mobileNo': '9431881414',
      'address': 'SHANTI VIHAR COLONY DIGHA PATNA',
      'totalDues': '2190617',
      'paymentStatus': 'Pending',
      'circle': 'Patliputra Circle',
      'wardNo': '1',
    },
    '1185006': {
      'pid': '1185006',
      'ownerName': 'SMT SOBHA DEVI',
      'guardianName': 'RAM EKBAL PRASAD',
      'mobileNo': '9199774991',
      'address': 'ALPANA CINEMA KE PICHHE DIGHA GHAT PATNA',
      'totalDues': '1902',
      'paymentStatus': 'Pending',
      'circle': 'Patliputra Circle',
      'wardNo': '1',
    },
  };

  ApiService() {
    _loadSettings();
  }

  Future<void> _loadSettings() async {
    final prefs = await SharedPreferences.getInstance();
    _baseUrl = prefs.getString(keyBaseUrl) ?? defaultBaseUrl;
    // Force local testing URL even if an old URL is cached
    if (_baseUrl.contains('arvixe.com')) {
      _baseUrl = defaultBaseUrl;
      await prefs.setString(keyBaseUrl, defaultBaseUrl); // Overwrite the cached value
    }
    // Connect to the real backend running locally!
    _isSimulated = false;
    await prefs.setBool(keyIsSimulated, false);

    // Load local activation log
    final activationsJson = prefs.getString('pmc_activations_log') ?? '[]';
    _recentActivations = List<Map<String, dynamic>>.from(
      jsonDecode(activationsJson),
    );

    // Load active session
    final sessionJson = prefs.getString(keyUserSession);
    if (sessionJson != null) {
      try {
        _currentUser = StaffUser.fromJson(jsonDecode(sessionJson));
      } catch (_) {
        prefs.remove(keyUserSession);
      }
    }

    // Load simulated data edits
    final localQrsJson = prefs.getString(keyLocalQrs);
    if (localQrsJson != null) {
      final localQrs = Map<String, dynamic>.from(jsonDecode(localQrsJson));
      localQrs.forEach((key, value) {
        _simulatedQrs[key] = Map<String, dynamic>.from(value);
      });
    }

    notifyListeners();
  }

  Future<void> setSimulated(bool val) async {
    _isSimulated = val;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(keyIsSimulated, val);
    notifyListeners();
  }

  Future<void> setBaseUrl(String url) async {
    _baseUrl = url.endsWith('/') ? url.substring(0, url.length - 1) : url;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(keyBaseUrl, _baseUrl);
    notifyListeners();
  }

  Future<void> _saveSimulatedQrs() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(keyLocalQrs, jsonEncode(_simulatedQrs));
  }

  Future<void> _saveActivationsLog() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(
      'pmc_activations_log',
      jsonEncode(_recentActivations),
    );
  }

  // ── HELPERS ───────────────────────────────────────────────────────────
  /// Safely decode JSON — returns null instead of throwing FormatException
  Map<String, dynamic>? _safeJsonDecode(String body) {
    try {
      return jsonDecode(body) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }

  // ── API: STAFF LOGIN ──────────────────────────────────────────────────

  Future<bool> login(String username, String password) async {
    _isLoading = true;
    notifyListeners();

    try {
      if (_isSimulated) {
        await Future.delayed(
          const Duration(milliseconds: 800),
        ); // Simulate delay
        if ((username.trim().toLowerCase() == 'admin' &&
                password == 'Admin@1234') ||
            (username.trim().toLowerCase() == 'emp101' &&
                password == 'password') ||
            (username.trim().toLowerCase() == 'tax101' &&
                password == 'taxpass')) {
          _currentUser = StaffUser(
            staffId: username.toLowerCase() == 'admin'
                ? 1
                : (username.toLowerCase() == 'tax101' ? 102 : 101),
            loginId: username,
            fullName: username.toLowerCase() == 'admin'
                ? 'System Admin'
                : username.toLowerCase() == 'tax101'
                ? 'Tax Collector 101'
                : 'EMP 101 (Field Agent)',
            role: username.toLowerCase() == 'admin' ? 'ADMIN' : 'FIELD_STAFF',
            wardNo: 1,
            circle: 'Patliputra Circle',
            mobile: '9999999999',
          );

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
          throw Exception('Invalid username or password');
        }
      } else {
        // Real HTTP Call — send as JSON to match LoginHandler.ashx expectation
        final forceUrl = defaultBaseUrl; // Bypass any broken SharedPreferences cache
        final url = Uri.parse('$forceUrl/LoginHandler.ashx');
        final payload = {'username': username.trim(), 'password': password};
        print('🔍 Staff login → $url');
        print('🔍 Payload: $payload');

        final response = await http
            .post(
              url,
              headers: {'Content-Type': 'application/json'},
              body: jsonEncode(payload),
            )
            .timeout(const Duration(seconds: 15));

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
          throw Exception(data?['message'] ?? 'Login failed');
        }
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
      final uri = Uri.parse('$defaultBaseUrl/CitizenPortal.ashx').replace(
        queryParameters: {
          'action': 'tax_login',
          'username': username,
          'password': password,
        },
      );
      final response = await http.get(uri).timeout(const Duration(seconds: 20));
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

  // ── API: LOOKUP QR CARD ────────────────────────────────────────────────
  Future<QrCard> lookupCard(String scannedValue) async {
    _isLoading = true;
    notifyListeners();

    try {
      if (_isSimulated) {
        await Future.delayed(const Duration(milliseconds: 500));

        String tokenKey = scannedValue.trim();
        // Parse token if URL was scanned
        if (tokenKey.startsWith('http')) {
          try {
            final uri = Uri.parse(tokenKey);
            tokenKey = uri.pathSegments.last;
          } catch (_) {}
        }

        // Search in local simulation map
        Map<String, dynamic>? match;
        if (_simulatedQrs.containsKey(tokenKey)) {
          match = _simulatedQrs[tokenKey];
        } else {
          // Search by QRId
          for (var item in _simulatedQrs.values) {
            if (item['qrId'] == scannedValue) {
              match = item;
              break;
            }
          }
        }

        _isLoading = false;
        notifyListeners();

        if (match != null) {
          return QrCard.fromJson(match);
        } else {
          throw Exception(
            'QR card details not found in system. Please verify if this card is registered.',
          );
        }
      } else {
        // Real HTTP Call
        final response = await http
            .get(
              Uri.parse(
                '$defaultBaseUrl/CardLookupHandler.ashx?value=${Uri.encodeComponent(scannedValue)}',
              ),
            )
            .timeout(const Duration(seconds: 10));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          return QrCard.fromJson(data['data']);
        } else {
          throw Exception(data['message'] ?? 'Card not found');
        }
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
      if (_isSimulated) {
        await Future.delayed(const Duration(milliseconds: 500));

        final cleanPid = pid.trim();
        if (_simulatedProperties.containsKey(cleanPid)) {
          _isLoading = false;
          notifyListeners();
          return Property.fromJson(_simulatedProperties[cleanPid]!);
        } else {
          _isLoading = false;
          notifyListeners();
          throw Exception(
            'Property ID (PID) not found in system records. Please verify the PID.',
          );
        }
      } else {
        // Real HTTP Call
        final response = await http
            .get(
              Uri.parse(
                '$defaultBaseUrl/PropertyLookupHandler.ashx?pid=${Uri.encodeComponent(pid)}',
              ),
            )
            .timeout(const Duration(seconds: 10));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          return Property.fromJson(data['data']);
        } else {
          throw Exception(data['message'] ?? 'Property not found');
        }
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
      if (_isSimulated) {
        await Future.delayed(const Duration(milliseconds: 500));
        _isLoading = false;
        notifyListeners();
        
        // Mock data for simulation
        if (searchQuery == '1409113' || searchQuery == '1234567') {
          return [
            PropertyDetailRow(
              pid: '1409113', applicationNo: 'SAS-1234', ownerName: 'SRI KUMUD VISHAL', guardianName: 'SRI BALMIKI PRASAD SINGH', address: '123 Mock Street', plotArea: '1500', constructedArea: '1200', assessmentYear: '2023', streetType: 'Main Road', floorNo: 'Ground Floor', builtupArea: '1200', useType: 'Residential', usageType: 'Self Occupied', contructionType: 'Pucca', occupancyType: 'Owner', mobileNo: '9999999999', revenueCircleNo: '268', circle: 'Bankipur', ward: '1'
            )
          ];
        } else {
          throw Exception('Extended details not found for PID.');
        }
      } else {
        final response = await http
            .get(Uri.parse('$defaultBaseUrl/TaxCollectorPropertyDetailsHandler.ashx?search=${Uri.encodeComponent(searchQuery)}'))
            .timeout(const Duration(seconds: 10));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          final List<dynamic> list = data['data'];
          return list.map((item) => PropertyDetailRow.fromJson(item)).toList();
        } else {
          throw Exception(data['message'] ?? 'Extended property details not found');
        }
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
      if (_isSimulated) {
        await Future.delayed(const Duration(milliseconds: 500));
        _isLoading = false;
        notifyListeners();

        // Mock data
        return [
          TaxHistoryRow(finYear: '2023-2024', propertyId: pid, floorTax: '1208.00', vacantTax: '0.00', totalTax: '1208.00'),
          TaxHistoryRow(finYear: '2024-2025', propertyId: pid, floorTax: '1208.00', vacantTax: '0.00', totalTax: '1208.00'),
          TaxHistoryRow(finYear: '2025-2026', propertyId: pid, floorTax: '1208.00', vacantTax: '0.00', totalTax: '1208.00'),
          TaxHistoryRow(finYear: '2026-2027', propertyId: pid, floorTax: '1208.00', vacantTax: '0.00', totalTax: '1208.00'),
          TaxHistoryRow(finYear: 'TOTAL', propertyId: pid, floorTax: '', vacantTax: '', totalTax: '4832.00'),
        ];
      } else {
        final timestamp = DateTime.now().millisecondsSinceEpoch;
        final response = await http
            .get(Uri.parse('$defaultBaseUrl/TaxHistoryHandler.ashx?pid=${Uri.encodeComponent(pid)}&_t=$timestamp'))
            .timeout(const Duration(seconds: 10));

        final data = jsonDecode(response.body);
        _isLoading = false;
        notifyListeners();

        if (response.statusCode == 200 && data['success'] == true) {
          final List<dynamic> list = data['data'];
          return list.map((item) => TaxHistoryRow.fromJson(item)).toList();
        } else {
          throw Exception(data['message'] ?? 'Tax history not found');
        }
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

      if (_isSimulated) {
        await Future.delayed(const Duration(seconds: 1));

        // 1. Verify card exists & check status in simulation
        if (!_simulatedQrs.containsKey(qrToken)) {
          throw Exception('QR card token not registered.');
        }

        final card = _simulatedQrs[qrToken]!;
        if (card['status'] == 'ACTIVATED') {
          throw Exception(
            'This QR card is already activated for PID: ${card['propertyId']}',
          );
        }

        // 2. Verify PID exists
        if (!_simulatedProperties.containsKey(pid)) {
          throw Exception('Property ID (PID) does not exist.');
        }

        // 3. Mark old active cards for this PID as REPLACED
        _simulatedQrs.forEach((key, value) {
          if (value['propertyId'] == pid && value['status'] == 'ACTIVATED') {
            value['status'] = 'REPLACED';
            value['activatedDate'] = DateTime.now().toString().substring(0, 19);
            value['activatedBy'] = staffId;
          }
        });

        // 4. Update card details
        card['status'] = 'ACTIVATED';
        card['propertyId'] = pid;
        card['activatedDate'] = DateTime.now().toString().substring(0, 19);
        card['activatedBy'] = staffId;

        await _saveSimulatedQrs();

        // Log locally
        final newLog = {
          'qrId': qrId,
          'pid': pid,
          'owner': _simulatedProperties[pid]!['ownerName'],
          'activatedAt': DateTime.now().toString().substring(0, 19),
        };
        _recentActivations.insert(0, newLog);
        await _saveActivationsLog();

        _isLoading = false;
        notifyListeners();
      } else {
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
              Uri.parse('$defaultBaseUrl/ActivateCardHandler.ashx'),
              headers: {'Content-Type': 'application/json'},
              body: jsonEncode({
                'qrid': qrId,
                'pid': pid,
                'staff_id': staffId,
                'device_id': 'flutter_device_agent',
                'geo_lat': geoLat,
                'geo_long': geoLong,
              }),
            )
            .timeout(const Duration(seconds: 12));

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
      if (_isSimulated) {
        await Future.delayed(const Duration(seconds: 1));
        return;
      }
      final response = await http.post(
        Uri.parse('$defaultBaseUrl/ProcessPaymentHandler.ashx'),
        headers: {'Content-Type': 'application/json'},
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
}
