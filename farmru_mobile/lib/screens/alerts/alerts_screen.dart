import 'package:flutter/material.dart';
import '../../models/alert_models.dart';
import '../../services/alert_service.dart';
import 'alert_detail_screen.dart';

class AlertsScreen extends StatefulWidget {
  const AlertsScreen({super.key});

  @override
  State<AlertsScreen> createState() => _AlertsScreenState();
}

class _AlertsScreenState extends State<AlertsScreen> {
  List<AlertItem>? alerts;
  bool loading = true;

  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    setState(() => loading = true);
    await AlertService.syncPendingActions();
    alerts = await AlertService.getActiveAlerts();
    setState(() => loading = false);
  }

  Color severityColor(int s) {
    switch (s) {
      case 2:
        return Colors.red;
      case 1:
        return Colors.orange;
      default:
        return Colors.blue;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text('Alerts', style: TextStyle(color: Colors.white)),
      ),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: load,
              child: ListView.builder(
                itemCount: alerts?.length ?? 0,
                itemBuilder: (context, i) {
                  final a = alerts![i];
                  return ListTile(
                    leading: Icon(Icons.warning, color: severityColor(a.severity)),
                    title: Text(a.title),
                    subtitle: Text('${a.nodeDisplay ?? ''} ${a.facilityDisplay ?? ''}'),
                    trailing: a.isResolved
                        ? const Icon(Icons.check, color: Colors.green)
                        : null,
                    onTap: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => AlertDetailScreen(alertId: a.id),
                      ),
                    ),
                  );
                },
              ),
            ),
    );
  }
}
