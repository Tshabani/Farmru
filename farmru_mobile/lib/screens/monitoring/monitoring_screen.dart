import 'package:flutter/material.dart';
import '../../services/monitoring_service.dart';
import '../alerts/alerts_screen.dart';

class MonitoringScreen extends StatefulWidget {
  const MonitoringScreen({super.key});

  @override
  State<MonitoringScreen> createState() => _MonitoringScreenState();
}

class _MonitoringScreenState extends State<MonitoringScreen> {
  MonitoringDashboard? dashboard;
  bool loading = true;

  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    setState(() => loading = true);
    dashboard = await MonitoringService.getDashboard();
    setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text('Operational monitoring', style: TextStyle(color: Colors.white)),
        actions: [
          IconButton(
            icon: const Icon(Icons.notifications_active, color: Colors.white),
            onPressed: () => Navigator.push(
              context,
              MaterialPageRoute(builder: (_) => const AlertsScreen()),
            ),
          ),
        ],
      ),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: load,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  if (dashboard != null) ...[
                    _card('Online devices', '${dashboard!.onlineDevices}', Colors.green),
                    _card('Offline devices', '${dashboard!.offlineDevices}', Colors.red),
                    _card('Stale telemetry', '${dashboard!.staleTelemetryDevices}', Colors.orange),
                    _card('Critical alerts', '${dashboard!.criticalAlerts}', Colors.red.shade900),
                    const SizedBox(height: 12),
                    Text(
                      dashboard!.monitoringEnabled ? 'Engine: enabled' : 'Engine: disabled',
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                    if (dashboard!.lastExecutionAt != null)
                      Text('Last run: ${dashboard!.lastExecutionAt}'),
                  ],
                ],
              ),
            ),
    );
  }

  Widget _card(String title, String value, Color color) {
    return Card(
      child: ListTile(
        leading: CircleAvatar(backgroundColor: color.withOpacity(0.2), child: Icon(Icons.sensors, color: color)),
        title: Text(title),
        trailing: Text(value, style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: color)),
      ),
    );
  }
}
