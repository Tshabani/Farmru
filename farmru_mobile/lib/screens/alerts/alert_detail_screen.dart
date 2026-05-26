import 'package:flutter/material.dart';
import '../../models/alert_models.dart';
import '../../services/alert_service.dart';

const _primary = Color(0xFFB7873B);

class AlertDetailScreen extends StatefulWidget {
  final String alertId;
  const AlertDetailScreen({super.key, required this.alertId});

  @override
  State<AlertDetailScreen> createState() => _AlertDetailScreenState();
}

class _AlertDetailScreenState extends State<AlertDetailScreen> {
  AlertItem? alert;
  bool loading = true;
  bool _actioning = false;
  final notesController = TextEditingController();

  @override
  void initState() {
    super.initState();
    load();
  }

  @override
  void dispose() {
    notesController.dispose();
    super.dispose();
  }

  Future<void> load() async {
    setState(() => loading = true);
    alert = await AlertService.getAlert(widget.alertId);
    if (mounted) setState(() => loading = false);
  }

  Color get _severityColor {
    switch (alert?.severity) {
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
      appBar: AppBar(title: const Text('Alert Detail')),
      body: loading || alert == null
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _HeaderCard(alert: alert!, color: _severityColor),
                  const SizedBox(height: 14),
                  if (alert!.description != null) ...[
                    _DescriptionCard(description: alert!.description!),
                    const SizedBox(height: 14),
                  ],
                  if (alert!.nodeDisplay != null ||
                      alert!.facilityDisplay != null)
                    _MetaCard(alert: alert!),
                  if (!alert!.isResolved) ...[
                    const SizedBox(height: 20),
                    const _SectionLabel('Resolution'),
                    const SizedBox(height: 10),
                    _ResolutionPanel(
                      alert: alert!,
                      controller: notesController,
                      actioning: _actioning,
                      onAcknowledge: () async {
                        setState(() => _actioning = true);
                        await AlertService.acknowledge(alert!.id);
                        await load();
                        setState(() => _actioning = false);
                      },
                      onResolve: () async {
                        setState(() => _actioning = true);
                        await AlertService.resolve(alert!.id,
                            notes: notesController.text);
                        if (context.mounted) Navigator.pop(context);
                      },
                    ),
                  ],
                  if (alert!.isResolved) ...[
                    const SizedBox(height: 14),
                    const _ResolvedBanner(),
                  ],
                ],
              ),
            ),
    );
  }
}

// ─── Header Card ──────────────────────────────────────────────────────────────

class _HeaderCard extends StatelessWidget {
  final AlertItem alert;
  final Color color;
  const _HeaderCard({required this.alert, required this.color});

  IconData get _icon {
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
    final surface = Theme.of(context).colorScheme.surface;
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(16),
        border: Border(left: BorderSide(color: color, width: 5)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.1),
              shape: BoxShape.circle,
            ),
            child: Icon(_icon, color: color, size: 22),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(alert.title,
                    style: const TextStyle(
                        fontSize: 16, fontWeight: FontWeight.bold)),
                const SizedBox(height: 6),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
                  decoration: BoxDecoration(
                    color: color.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Text(
                    alert.severityLabel,
                    style: TextStyle(
                        fontSize: 11,
                        color: color,
                        fontWeight: FontWeight.w600),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Description Card ─────────────────────────────────────────────────────────

class _DescriptionCard extends StatelessWidget {
  final String description;
  const _DescriptionCard({required this.description});

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Container(
      width: double.infinity,
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
          Text('Description',
              style: TextStyle(
                  fontWeight: FontWeight.w700,
                  fontSize: 12,
                  color: onSurface.withValues(alpha: 0.45),
                  letterSpacing: 0.6)),
          const SizedBox(height: 8),
          Text(description,
              style: const TextStyle(fontSize: 14, height: 1.5)),
        ],
      ),
    );
  }
}

// ─── Meta Card ────────────────────────────────────────────────────────────────

class _MetaCard extends StatelessWidget {
  final AlertItem alert;
  const _MetaCard({required this.alert});

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
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
        children: [
          if (alert.nodeDisplay?.isNotEmpty == true)
            _MetaRow(
                icon: Icons.sensors_rounded,
                label: 'Node',
                value: alert.nodeDisplay!),
          if (alert.facilityDisplay?.isNotEmpty == true)
            _MetaRow(
                icon: Icons.business_rounded,
                label: 'Facility',
                value: alert.facilityDisplay!),
          if (alert.triggeredAt != null)
            _MetaRow(
                icon: Icons.schedule_rounded,
                label: 'Triggered',
                value: alert.triggeredAt!),
        ],
      ),
    );
  }
}

class _MetaRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  const _MetaRow(
      {required this.icon, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Icon(icon, size: 16, color: onSurface.withValues(alpha: 0.38)),
          const SizedBox(width: 10),
          Text(label,
              style: TextStyle(
                  color: onSurface.withValues(alpha: 0.45), fontSize: 12)),
          const Spacer(),
          Text(value,
              style: const TextStyle(
                  fontWeight: FontWeight.w500, fontSize: 13)),
        ],
      ),
    );
  }
}

// ─── Resolution Panel ─────────────────────────────────────────────────────────

class _ResolutionPanel extends StatelessWidget {
  final AlertItem alert;
  final TextEditingController controller;
  final bool actioning;
  final VoidCallback onAcknowledge;
  final VoidCallback onResolve;

  const _ResolutionPanel({
    required this.alert,
    required this.controller,
    required this.actioning,
    required this.onAcknowledge,
    required this.onResolve,
  });

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
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
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextField(
            controller: controller,
            maxLines: 3,
            decoration: InputDecoration(
              hintText: 'Add resolution notes…',
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(10),
                borderSide: BorderSide.none,
              ),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
          const SizedBox(height: 14),
          if (actioning)
            const Center(child: CircularProgressIndicator(color: _primary))
          else
            Row(
              children: [
                if (!alert.isAcknowledged) ...[
                  Expanded(
                    child: OutlinedButton.icon(
                      onPressed: onAcknowledge,
                      icon: const Icon(Icons.thumb_up_rounded, size: 16),
                      label: const Text('Acknowledge'),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: _primary,
                        side: const BorderSide(color: _primary),
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(10)),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                ],
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: onResolve,
                    icon: const Icon(Icons.check_rounded, size: 16),
                    label: const Text('Resolve'),
                    style: ElevatedButton.styleFrom(
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10)),
                    ),
                  ),
                ),
              ],
            ),
        ],
      ),
    );
  }
}

// ─── Resolved Banner ──────────────────────────────────────────────────────────

class _ResolvedBanner extends StatelessWidget {
  const _ResolvedBanner();

  @override
  Widget build(BuildContext context) => Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: Colors.green.withValues(alpha: 0.08),
          border: Border.all(color: Colors.green.withValues(alpha: 0.3)),
          borderRadius: BorderRadius.circular(14),
        ),
        child: const Row(
          children: [
            Icon(Icons.check_circle_rounded, color: Colors.green, size: 28),
            SizedBox(width: 12),
            Text('This alert has been resolved.',
                style: TextStyle(
                    color: Colors.green,
                    fontWeight: FontWeight.w600,
                    fontSize: 14)),
          ],
        ),
      );
}

// ─── Section Label ────────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Text(
        text,
        style: TextStyle(
          fontSize: 13,
          fontWeight: FontWeight.w700,
          color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54),
          letterSpacing: 0.8,
        ),
      );
}
