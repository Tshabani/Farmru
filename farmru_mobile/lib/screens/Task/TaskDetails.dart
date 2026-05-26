import 'package:flutter/material.dart';
import '../../models/task_models.dart';
import '../../services/task_service.dart';

const _primary = Color(0xFFB7873B);

class TaskDetailsPage extends StatefulWidget {
  final TaskItem task;
  const TaskDetailsPage({super.key, required this.task});

  @override
  State<TaskDetailsPage> createState() => _TaskDetailsPageState();
}

class _TaskDetailsPageState extends State<TaskDetailsPage> {
  bool _actioning = false;

  Color get _statusColor {
    switch (widget.task.status) {
      case 1:
        return Colors.blue;
      case 2:
        return Colors.orange;
      case 3:
        return Colors.green;
      default:
        return Colors.grey;
    }
  }

  Future<void> _updateStatus(int status) async {
    setState(() => _actioning = true);
    await TaskService.updateStatus(widget.task.id, status);
    if (mounted) Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    final task = widget.task;
    return Scaffold(
      appBar: AppBar(title: const Text('Task Detail')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _HeaderCard(task: task, statusColor: _statusColor),
            if (task.description?.isNotEmpty == true) ...[
              const SizedBox(height: 14),
              _DescriptionCard(description: task.description!),
            ],
            const SizedBox(height: 14),
            _MetaCard(task: task),
            if (task.status != 3) ...[
              const SizedBox(height: 20),
              const _SectionLabel('Actions'),
              const SizedBox(height: 10),
              _ActionsPanel(
                task: task,
                actioning: _actioning,
                onStartWork: task.status == 1
                    ? () => _updateStatus(2)
                    : null,
                onClose: () => _updateStatus(3),
              ),
            ],
            if (task.status == 3) ...[
              const SizedBox(height: 14),
              _ClosedBanner(),
            ],
          ],
        ),
      ),
    );
  }
}

// ─── Header Card ──────────────────────────────────────────────────────────────

class _HeaderCard extends StatelessWidget {
  final TaskItem task;
  final Color statusColor;
  const _HeaderCard({required this.task, required this.statusColor});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(16),
        border: Border(left: BorderSide(color: statusColor, width: 5)),
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
              color: statusColor.withValues(alpha: 0.1),
              shape: BoxShape.circle,
            ),
            child: Icon(Icons.task_alt_rounded, color: statusColor, size: 22),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(task.title,
                    style: const TextStyle(
                        fontSize: 16, fontWeight: FontWeight.bold)),
                const SizedBox(height: 6),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
                  decoration: BoxDecoration(
                    color: statusColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Text(
                    task.statusLabel,
                    style: TextStyle(
                        fontSize: 11,
                        color: statusColor,
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
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
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
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.45),
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
  final TaskItem task;
  const _MetaCard({required this.task});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
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
          if (task.assignedToName?.isNotEmpty == true)
            _MetaRow(
                icon: Icons.person_rounded,
                label: 'Assigned to',
                value: task.assignedToName!),
          if (task.assignedByName?.isNotEmpty == true)
            _MetaRow(
                icon: Icons.supervisor_account_rounded,
                label: 'Assigned by',
                value: task.assignedByName!),
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
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Icon(icon, size: 16,
              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.38)),
          const SizedBox(width: 10),
          Text(label,
              style: TextStyle(
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.45),
                  fontSize: 12)),
          const Spacer(),
          Text(value,
              style: const TextStyle(
                  fontWeight: FontWeight.w500, fontSize: 13)),
        ],
      ),
    );
  }
}

// ─── Actions Panel ────────────────────────────────────────────────────────────

class _ActionsPanel extends StatelessWidget {
  final TaskItem task;
  final bool actioning;
  final VoidCallback? onStartWork;
  final VoidCallback onClose;

  const _ActionsPanel({
    required this.task,
    required this.actioning,
    required this.onStartWork,
    required this.onClose,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: actioning
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : Row(
              children: [
                if (onStartWork != null) ...[
                  Expanded(
                    child: OutlinedButton.icon(
                      onPressed: onStartWork,
                      icon: const Icon(Icons.play_arrow_rounded, size: 16),
                      label: const Text('Start Work'),
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
                    onPressed: onClose,
                    icon: const Icon(Icons.check_rounded, size: 16),
                    label: const Text('Close Task'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _primary,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10)),
                    ),
                  ),
                ),
              ],
            ),
    );
  }
}

// ─── Closed Banner ────────────────────────────────────────────────────────────

class _ClosedBanner extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
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
          Text('This task has been closed.',
              style: TextStyle(
                  color: Colors.green,
                  fontWeight: FontWeight.w600,
                  fontSize: 14)),
        ],
      ),
    );
  }
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
