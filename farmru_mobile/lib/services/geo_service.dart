import 'dart:convert';
import '../utils/base_client.dart';

class GeoService {
  static Future<Map<String, dynamic>?> getOperationalMap() async {
    final response = await BaseClient().get('api/services/app/GeoSpatial/GetOperationalMap');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return decoded['result'] as Map<String, dynamic>?;
  }

  static Future<Map<String, dynamic>?> getExecutiveSummary() async {
    final response = await BaseClient().get('api/services/app/GeoSpatial/GetExecutiveSummary');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return decoded['result'] as Map<String, dynamic>?;
  }
}
