import 'dart:convert';
import '../models/incident_models.dart';
import '../utils/base_client.dart';

class IncidentService {
  static final List<Map<String, dynamic>> _pending = [];

  static Future<List<IncidentItem>?> getMyAssignedIncidents() async {
    final response = await BaseClient()
        .get('api/services/app/Incident/GetMyAssignedIncidents');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    final list = decoded['result'] as List<dynamic>? ?? [];
    return list.map((e) => IncidentItem.fromJson(e)).toList();
  }

  static Future<List<IncidentItem>?> getActiveIncidents() async {
    final response =
        await BaseClient().get('api/services/app/Incident/GetActiveIncidents');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    final list = decoded['result'] as List<dynamic>? ?? [];
    return list.map((e) => IncidentItem.fromJson(e)).toList();
  }

  static Future<IncidentDetail?> getIncident(String id) async {
    final response =
        await BaseClient().get('api/services/app/Incident/GetIncident?Id=$id');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return IncidentDetail.fromJson(decoded['result']);
  }

  static Future<bool> acknowledge(String incidentId) async {
    return _post('Acknowledge', {'incidentId': incidentId});
  }

  static Future<bool> startWork(String incidentId) async {
    return _post('StartWork', {'incidentId': incidentId});
  }

  static Future<bool> resolve(String incidentId, {String? notes}) async {
    return _post('Resolve', {'incidentId': incidentId, 'notes': notes});
  }

  static Future<bool> escalate(String incidentId, {String? notes}) async {
    return _post('Escalate', {'incidentId': incidentId, 'notes': notes});
  }

  static Future<bool> addComment(String incidentId, String comment) async {
    return _post('AddComment', {'incidentId': incidentId, 'comment': comment},
        voidResponse: true);
  }

  static Future<bool> _post(String action, Map<String, dynamic> body,
      {bool voidResponse = false}) async {
    try {
      final response = await BaseClient()
          .post('api/services/app/Incident/$action', jsonEncode(body));
      return response != null;
    } catch (_) {
      _pending.add({'action': action, ...body});
      return false;
    }
  }

  static Future<void> syncPending() async {
    final copy = List<Map<String, dynamic>>.from(_pending);
    _pending.clear();
    for (final item in copy) {
      final action = item.remove('action');
      await _post(action, item);
    }
  }
}
