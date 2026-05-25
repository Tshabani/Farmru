import 'package:flutter/material.dart';
import '../../models/alert_models.dart';
import '../../services/alert_service.dart';

class AlertDetailScreen extends StatefulWidget {
  final String alertId;
  const AlertDetailScreen({super.key, required this.alertId});

  @override
  State<AlertDetailScreen> createState() => _AlertDetailScreenState();
}

class _AlertDetailScreenState extends State<AlertDetailScreen> {
  AlertItem? alert;
  bool loading = true;
  final notesController = TextEditingController();

  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    alert = await AlertService.getAlert(widget.alertId);
    setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text('Alert', style: TextStyle(color: Colors.white)),
      ),
      body: loading || alert == null
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(alert!.title, style: Theme.of(context).textTheme.titleLarge),
                  const SizedBox(height: 8),
                  Text(alert!.severityLabel),
                  if (alert!.description != null) Text(alert!.description!),
                  const SizedBox(height: 16),
                  if (!alert!.isResolved) ...[
                    TextField(
                      controller: notesController,
                      decoration: const InputDecoration(labelText: 'Resolution notes'),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        if (!alert!.isAcknowledged)
                          ElevatedButton(
                            onPressed: () async {
                              await AlertService.acknowledge(alert!.id);
                              load();
                            },
                            child: const Text('Acknowledge'),
                          ),
                        const SizedBox(width: 8),
                        ElevatedButton(
                          onPressed: () async {
                            await AlertService.resolve(alert!.id,
                                notes: notesController.text);
                            Navigator.pop(context);
                          },
                          child: const Text('Resolve'),
                        ),
                      ],
                    ),
                  ],
                ],
              ),
            ),
    );
  }
}
