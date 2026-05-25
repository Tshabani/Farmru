import 'dart:convert';
import '../models/alert_models.dart';
import '../utils/base_client.dart';

class AlertService {
  static final List<Map<String, dynamic>> _pendingActions = [];

  static Future<List<AlertItem>?> getActiveAlerts() async {
    final response = await BaseClient().get('api/services/app/Alert/GetActiveAlerts');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    final list = decoded['result'] as List<dynamic>? ?? [];
    return list.map((e) => AlertItem.fromJson(e)).toList();
  }

  static Future<List<AlertItem>?> getAlertsByNode(String nodeId) async {
    final response = await BaseClient()
        .get('api/services/app/Alert/GetAlertsByNode?Id=$nodeId');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    final list = decoded['result'] as List<dynamic>? ?? [];
    return list.map((e) => AlertItem.fromJson(e)).toList();
  }

  static Future<AlertItem?> getAlert(String id) async {
    final response = await BaseClient().get('api/services/app/Alert/GetAlert?Id=$id');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return AlertItem.fromJson(decoded['result']);
  }

  static Future<bool> acknowledge(String alertId) async {
    return _postAction('Acknowledge', {'alertId': alertId});
  }

  static Future<bool> resolve(String alertId, {String? notes}) async {
    return _postAction('Resolve', {'alertId': alertId, 'resolutionNotes': notes});
  }

  static Future<bool> _postAction(String action, Map<String, dynamic> body) async {
    try {
      final response = await BaseClient().post(
          'api/services/app/Alert/$action', jsonEncode(body));
      return response != null;
    } catch (_) {
      _pendingActions.add({'action': action, ...body});
      return false;
    }
  }

  static Future<void> syncPendingActions() async {
    final copy = List<Map<String, dynamic>>.from(_pendingActions);
    _pendingActions.clear();
    for (final item in copy) {
      final action = item.remove('action');
      await _postAction(action, item);
    }
  }
}
