import 'package:flutter/material.dart';
import '../../models/incident_models.dart';
import '../../services/incident_service.dart';
import 'IncidentsDetails.dart';

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
    'Open',
    'Assigned',
    'In progress',
    'Waiting',
    'Escalated',
    'Resolved',
    'Closed',
    'Cancelled'
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Field incidents'),
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _load),
        ],
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _load,
              child: ListView(
                padding: const EdgeInsets.all(12),
                children: [
                  const Text('My assignments',
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  ...(_assigned ?? []).map(_tile),
                  const SizedBox(height: 16),
                  const Text('Nearby / active',
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  ...(_active ?? [])
                      .where((i) => !(_assigned ?? []).any((a) => a.id == i.id))
                      .take(20)
                      .map(_tile),
                ],
              ),
            ),
    );
  }

  Widget _tile(IncidentItem item) {
    return Card(
      child: ListTile(
        title: Text(item.title),
        subtitle: Text(
            '${_statusLabels[item.status.clamp(0, _statusLabels.length - 1)]} · SLA ${item.slaStatus}'),
        trailing: const Icon(Icons.chevron_right),
        onTap: () => Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => IncidentDetailsPage(incidentId: item.id),
          ),
        ),
      ),
    );
  }
}
