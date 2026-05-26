import 'package:flutter/material.dart';
import '../../services/monitoring_service.dart';
import '../alerts/alerts_screen.dart';

const _primary = Color(0xFFB7873B);

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
    if (mounted) setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Monitoring'),
        actions: [
          IconButton(
            icon: const Icon(Icons.notifications_active_rounded),
            onPressed: () => Navigator.push(context,
                MaterialPageRoute(builder: (_) => const AlertsScreen())),
          ),
        ],
      ),
      body: loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: load,
              color: _primary,
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.all(16),
                child: dashboard == null
                    ? const _EmptyState()
                    : Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          _EngineStatusBanner(dashboard: dashboard!),
                          const SizedBox(height: 20),
                          const _SectionLabel('Device Health'),
                          const SizedBox(height: 12),
                          Row(
                            children: [
                              Expanded(
                                child: _StatCard(
                                  icon: Icons.wifi_rounded,
                                  label: 'Online',
                                  value: '${dashboard!.onlineDevices}',
                                  color: Colors.green,
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: _StatCard(
                                  icon: Icons.wifi_off_rounded,
                                  label: 'Offline',
                                  value: '${dashboard!.offlineDevices}',
                                  color: Colors.grey,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 12),
                          Row(
                            children: [
                              Expanded(
                                child: _StatCard(
                                  icon: Icons.schedule_rounded,
                                  label: 'Stale Telemetry',
                                  value:
                                      '${dashboard!.staleTelemetryDevices}',
                                  color: Colors.orange,
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: _StatCard(
                                  icon: Icons.error_rounded,
                                  label: 'Critical Alerts',
                                  value: '${dashboard!.criticalAlerts}',
                                  color: Colors.red,
                                ),
                              ),
                            ],
                          ),
                          if (dashboard!.lastExecutionAt != null) ...[
                            const SizedBox(height: 20),
                            const _SectionLabel('Engine Info'),
                            const SizedBox(height: 12),
                            _InfoRow(
                              label: 'Last run',
                              value: dashboard!.lastExecutionAt!,
                            ),
                          ],
                        ],
                      ),
              ),
            ),
    );
  }
}

// ─── Engine Status Banner ─────────────────────────────────────────────────────

class _EngineStatusBanner extends StatelessWidget {
  final MonitoringDashboard dashboard;
  const _EngineStatusBanner({required this.dashboard});

  @override
  Widget build(BuildContext context) {
    final enabled = dashboard.monitoringEnabled;
    final color = enabled ? Colors.green : Colors.orange;
    final label = enabled
        ? 'Monitoring Engine Active'
        : 'Monitoring Engine Disabled';
    final icon =
        enabled ? Icons.check_circle_rounded : Icons.pause_circle_rounded;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.08),
        border: Border.all(color: color.withValues(alpha: 0.3)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Row(
        children: [
          Icon(icon, color: color, size: 32),
          const SizedBox(width: 14),
          Expanded(
            child: Text(
              label,
              style: TextStyle(
                fontWeight: FontWeight.w600,
                fontSize: 15,
                color: color.withValues(alpha: 0.85),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Stat Card ────────────────────────────────────────────────────────────────

class _StatCard extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final Color color;

  const _StatCard({
    required this.icon,
    required this.label,
    required this.value,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(icon, color: color, size: 20),
          ),
          const SizedBox(height: 12),
          Text(value,
              style: TextStyle(
                  fontSize: 28, fontWeight: FontWeight.bold, color: color)),
          const SizedBox(height: 2),
          Text(label,
              style: TextStyle(
                  fontSize: 12, color: onSurface.withValues(alpha: 0.45))),
        ],
      ),
    );
  }
}

// ─── Info Row ─────────────────────────────────────────────────────────────────

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;
  const _InfoRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(14),
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
          Text(label,
              style: TextStyle(
                  color: onSurface.withValues(alpha: 0.45), fontSize: 13)),
          const Spacer(),
          Text(value,
              style: const TextStyle(
                  fontWeight: FontWeight.w500, fontSize: 13)),
        ],
      ),
    );
  }
}

// ─── Shared helpers ───────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Text(
        text,
        style: TextStyle(
          fontSize: 13,
          fontWeight: FontWeight.w700,
          color:
              Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54),
          letterSpacing: 0.8,
        ),
      );
}

class _EmptyState extends StatelessWidget {
  const _EmptyState();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          children: [
            Icon(Icons.monitor_heart_rounded,
                size: 48, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 12),
            Text('No monitoring data',
                style: TextStyle(color: onSurface.withValues(alpha: 0.38))),
          ],
        ),
      ),
    );
  }
}
