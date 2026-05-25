import 'dart:convert';
import '../utils/base_client.dart';

class MonitoringDashboard {
  final int onlineDevices;
  final int offlineDevices;
  final int staleTelemetryDevices;
  final int activeAlerts;
  final int criticalAlerts;
  final bool monitoringEnabled;
  final String? lastExecutionAt;

  MonitoringDashboard({
    required this.onlineDevices,
    required this.offlineDevices,
    required this.staleTelemetryDevices,
    required this.activeAlerts,
    required this.criticalAlerts,
    required this.monitoringEnabled,
    this.lastExecutionAt,
  });

  factory MonitoringDashboard.fromJson(Map<String, dynamic> json) => MonitoringDashboard(
        onlineDevices: json['onlineDevices'] ?? 0,
        offlineDevices: json['offlineDevices'] ?? 0,
        staleTelemetryDevices: json['staleTelemetryDevices'] ?? 0,
        activeAlerts: json['activeAlerts'] ?? 0,
        criticalAlerts: json['criticalAlerts'] ?? 0,
        monitoringEnabled: json['monitoringEnabled'] ?? true,
        lastExecutionAt: json['lastExecutionAt'],
      );
}

class MonitoringService {
  static Future<MonitoringDashboard?> getDashboard() async {
    final response = await BaseClient().get('api/services/app/Monitoring/GetDashboard');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return MonitoringDashboard.fromJson(decoded['result']);
  }
}
