import 'package:flutter/material.dart';
import '../../models/alert_models.dart';
import '../../services/alert_service.dart';
import 'alert_detail_screen.dart';

const _primary = Color(0xFFB7873B);

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
    if (mounted) setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Alerts')),
      body: loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: load,
              color: _primary,
              child: alerts == null || alerts!.isEmpty
                  ? const _EmptyAlerts()
                  : ListView.separated(
                      padding: const EdgeInsets.all(16),
                      itemCount: alerts!.length,
                      separatorBuilder: (_, __) => const SizedBox(height: 10),
                      itemBuilder: (context, i) => _AlertCard(
                        alert: alerts![i],
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) =>
                                AlertDetailScreen(alertId: alerts![i].id),
                          ),
                        ),
                      ),
                    ),
            ),
    );
  }
}

class _AlertCard extends StatelessWidget {
  final AlertItem alert;
  final VoidCallback onTap;
  const _AlertCard({required this.alert, required this.onTap});

  Color get _severityColor {
    switch (alert.severity) {
      case 2:
        return Colors.red;
      case 1:
        return Colors.orange;
      default:
        return Colors.blue;
    }
  }

  IconData get _severityIcon {
    switch (alert.severity) {
      case 2:
        return Icons.error_rounded;
      case 1:
        return Icons.warning_rounded;
      default:
        return Icons.info_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _severityColor;
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: surface,
          borderRadius: BorderRadius.circular(14),
          border: alert.isResolved
              ? null
              : Border(left: BorderSide(color: color, width: 4)),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 8,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.1),
                shape: BoxShape.circle,
              ),
              child: Icon(_severityIcon, color: color, size: 20),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(alert.title,
                      style: const TextStyle(
                          fontWeight: FontWeight.w600, fontSize: 14)),
                  const SizedBox(height: 3),
                  Text(
                    [
                      if (alert.nodeDisplay?.isNotEmpty == true)
                        alert.nodeDisplay!,
                      if (alert.facilityDisplay?.isNotEmpty == true)
                        alert.facilityDisplay!,
                    ].join(' · '),
                    style: TextStyle(
                        fontSize: 11,
                        color: onSurface.withValues(alpha: 0.45)),
                  ),
                  const SizedBox(height: 4),
                  _SeverityChip(label: alert.severityLabel, color: color),
                ],
              ),
            ),
            const SizedBox(width: 8),
            if (alert.isResolved)
              const Icon(Icons.check_circle_rounded,
                  color: Colors.green, size: 22)
            else
              Icon(Icons.chevron_right_rounded,
                  color: onSurface.withValues(alpha: 0.26)),
          ],
        ),
      ),
    );
  }
}

class _SeverityChip extends StatelessWidget {
  final String label;
  final Color color;
  const _SeverityChip({required this.label, required this.color});

  @override
  Widget build(BuildContext context) => Container(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(20),
        ),
        child: Text(label,
            style: TextStyle(
                fontSize: 10, color: color, fontWeight: FontWeight.w600)),
      );
}

class _EmptyAlerts extends StatelessWidget {
  const _EmptyAlerts();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.notifications_off_rounded,
                size: 56, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 16),
            Text('No active alerts',
                style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: onSurface.withValues(alpha: 0.38))),
            const SizedBox(height: 6),
            Text('All systems are operating normally.',
                style: TextStyle(
                    fontSize: 12, color: onSurface.withValues(alpha: 0.26))),
          ],
        ),
      ),
    );
  }
}
