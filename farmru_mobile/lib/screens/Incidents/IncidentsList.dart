import 'package:flutter/material.dart';
import '../../models/incident_models.dart';
import '../../services/incident_service.dart';
import 'IncidentsDetails.dart';

const _primary = Color(0xFFB7873B);

class IncidentListPage extends StatefulWidget {
  const IncidentListPage({super.key});

  @override
  State<IncidentListPage> createState() => _IncidentListPageState();
}

class _IncidentListPageState extends State<IncidentListPage> {
  List<IncidentItem>? _assigned;
  List<IncidentItem>? _active;
  bool _loading = true;

  static const _statusLabels = [
    'Open', 'Assigned', 'In Progress', 'Waiting',
    'Escalated', 'Resolved', 'Closed', 'Cancelled'
  ];

  static const _statusColors = [
    Colors.orange, Colors.blue, Colors.indigo, Colors.amber,
    Colors.red, Colors.green, Colors.grey, Colors.grey
  ];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    await IncidentService.syncPending();
    final assigned = await IncidentService.getMyAssignedIncidents();
    final active = await IncidentService.getActiveIncidents();
    if (!mounted) return;
    setState(() {
      _assigned = assigned ?? [];
      _active = active ?? [];
      _loading = false;
    });
  }

  String _statusLabel(int status) =>
      _statusLabels[status.clamp(0, _statusLabels.length - 1)];

  Color _statusColor(int status) =>
      _statusColors[status.clamp(0, _statusColors.length - 1)];

  @override
  Widget build(BuildContext context) {
    final uniqActive = (_active ?? [])
        .where((i) => !(_assigned ?? []).any((a) => a.id == i.id))
        .take(20)
        .toList();

    return Scaffold(
      appBar: AppBar(title: const Text('Field Incidents')),
      body: _loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: _load,
              color: _primary,
              child: (_assigned!.isEmpty && uniqActive.isEmpty)
                  ? const _EmptyIncidents()
                  : ListView(
                      padding: const EdgeInsets.all(16),
                      children: [
                        if (_assigned!.isNotEmpty) ...[
                          _SectionLabel(
                              'My Assignments', _assigned!.length),
                          const SizedBox(height: 10),
                          ..._assigned!.map((i) => _IncidentCard(
                                item: i,
                                statusLabel: _statusLabel(i.status),
                                statusColor: _statusColor(i.status),
                                onTap: () => _openDetail(i),
                              )),
                          const SizedBox(height: 20),
                        ],
                        if (uniqActive.isNotEmpty) ...[
                          _SectionLabel('Active Incidents', uniqActive.length),
                          const SizedBox(height: 10),
                          ...uniqActive.map((i) => _IncidentCard(
                                item: i,
                                statusLabel: _statusLabel(i.status),
                                statusColor: _statusColor(i.status),
                                onTap: () => _openDetail(i),
                              )),
                        ],
                      ],
                    ),
            ),
    );
  }

  void _openDetail(IncidentItem item) {
    Navigator.push(
      context,
      MaterialPageRoute(
          builder: (_) => IncidentDetailsPage(incidentId: item.id)),
    );
  }
}

// ─── Section Label ────────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String title;
  final int count;
  const _SectionLabel(this.title, this.count);

  @override
  Widget build(BuildContext context) => Padding(
        padding: const EdgeInsets.only(left: 2, bottom: 2),
        child: Text(
          '$title ($count)',
          style: TextStyle(
            fontSize: 13,
            fontWeight: FontWeight.w700,
            color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54),
            letterSpacing: 0.5,
          ),
        ),
      );
}

// ─── Incident Card ────────────────────────────────────────────────────────────

class _IncidentCard extends StatelessWidget {
  final IncidentItem item;
  final String statusLabel;
  final Color statusColor;
  final VoidCallback onTap;

  const _IncidentCard({
    required this.item,
    required this.statusLabel,
    required this.statusColor,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.only(bottom: 10),
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Theme.of(context).colorScheme.surface,
          borderRadius: BorderRadius.circular(14),
          border: Border(left: BorderSide(color: statusColor, width: 4)),
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
                color: statusColor.withValues(alpha: 0.1),
                shape: BoxShape.circle,
              ),
              child: Icon(Icons.report_problem_rounded,
                  color: statusColor, size: 20),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(item.title,
                      style: const TextStyle(
                          fontWeight: FontWeight.w600, fontSize: 14)),
                  const SizedBox(height: 5),
                  Row(
                    children: [
                      _StatusChip(label: statusLabel, color: statusColor),
                      const SizedBox(width: 6),
                      Text('SLA: ${item.slaStatus}',
                          style: TextStyle(
                              fontSize: 11,
                              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.45))),
                    ],
                  ),
                ],
              ),
            ),
            Icon(Icons.chevron_right_rounded,
                color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.26)),
          ],
        ),
      ),
    );
  }
}

class _StatusChip extends StatelessWidget {
  final String label;
  final Color color;
  const _StatusChip({required this.label, required this.color});

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

// ─── Empty State ──────────────────────────────────────────────────────────────

class _EmptyIncidents extends StatelessWidget {
  const _EmptyIncidents();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.check_circle_outline_rounded,
                size: 56, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 16),
            Text('No active incidents',
                style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: onSurface.withValues(alpha: 0.38))),
            const SizedBox(height: 6),
            Text('All clear — no incidents to show.',
                style: TextStyle(
                    fontSize: 12, color: onSurface.withValues(alpha: 0.26))),
          ],
        ),
      ),
    );
  }
}
