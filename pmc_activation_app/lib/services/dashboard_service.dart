import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/dashboard_models.dart';
import 'api_service.dart';

class DashboardService {
  final ApiService _api = ApiService(); // reuse baseUrl and simulated flag

  Future<ActivationStats> fetchStats() async {
    final url = '${_api.baseUrl}/GetActivationStats';
    final response = await http
        .get(Uri.parse(url))
        .timeout(const Duration(seconds: 15));
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return ActivationStats.fromJson(data);
    } else {
      throw Exception('Failed to load activation stats');
    }
  }

  Future<List<ActivationEvent>> fetchRecentEvents() async {
    final url = '${_api.baseUrl}/GetRecentActivations';
    final response = await http
        .get(Uri.parse(url))
        .timeout(const Duration(seconds: 15));
    if (response.statusCode == 200) {
      final List<dynamic> list = jsonDecode(response.body);
      return list.map((e) => ActivationEvent.fromJson(e)).toList();
    } else {
      throw Exception('Failed to load recent activations');
    }
  }
}
