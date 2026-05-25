import 'package:flutter/material.dart';
import '../../models/incident_models.dart';
import '../../services/incident_service.dart';

class IncidentDetailsPage extends StatefulWidget {
  final String incidentId;
  const IncidentDetailsPage({super.key, required this.incidentId});

  @override
  State<IncidentDetailsPage> createState() => _IncidentDetailsPageState();
}

class _IncidentDetailsPageState extends State<IncidentDetailsPage> {
  IncidentDetail? _detail;
  bool _loading = true;
  final _comment = TextEditingController();
  final _resolveNotes = TextEditingController();

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    final d = await IncidentService.getIncident(widget.incidentId);
    if (!mounted) return;
    setState(() {
      _detail = d;
      _loading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Incident')),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _detail == null
              ? const Center(child: Text('Not found'))
              : SingleChildScrollView(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(_detail!.title,
                          style: Theme.of(context).textTheme.titleLarge),
                      if (_detail!.description != null)
                        Text(_detail!.description!),
                      const SizedBox(height: 12),
                      Wrap(
                        spacing: 8,
                        children: [
                          ElevatedButton(
                            onPressed: () async {
                              await IncidentService.acknowledge(_detail!.id);
                              _load();
                            },
                            child: const Text('Acknowledge'),
                          ),
                          ElevatedButton(
                            onPressed: () async {
                              await IncidentService.startWork(_detail!.id);
                              _load();
                            },
                            child: const Text('Start work'),
                          ),
                          OutlinedButton(
                            onPressed: () async {
                              await IncidentService.escalate(_detail!.id,
                                  notes: _comment.text);
                              _load();
                            },
                            child: const Text('Escalate'),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),
                      TextField(
                        controller: _resolveNotes,
                        decoration:
                            const InputDecoration(labelText: 'Resolution notes'),
                      ),
                      ElevatedButton(
                        onPressed: () async {
                          await IncidentService.resolve(_detail!.id,
                              notes: _resolveNotes.text);
                          _load();
                        },
                        child: const Text('Resolve'),
                      ),
                      const SizedBox(height: 12),
                      TextField(
                        controller: _comment,
                        decoration: const InputDecoration(labelText: 'Comment'),
                      ),
                      TextButton(
                        onPressed: () async {
                          await IncidentService.addComment(
                              _detail!.id, _comment.text);
                          _comment.clear();
                          _load();
                        },
                        child: const Text('Add comment'),
                      ),
                      if (_detail!.latitude != null &&
                          _detail!.longitude != null)
                        Text(
                            'Location: ${_detail!.latitude}, ${_detail!.longitude}'),
                      const Divider(),
                      const Text('Timeline',
                          style: TextStyle(fontWeight: FontWeight.bold)),
                      ..._detail!.timeline.map((e) => ListTile(
                            title: Text(e.title),
                            subtitle: Text(e.description ?? e.creationTime),
                          )),
                    ],
                  ),
                ),
    );
  }
}
