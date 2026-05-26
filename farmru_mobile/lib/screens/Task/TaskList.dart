import 'package:flutter/material.dart';
import '../../models/task_models.dart';
import '../../services/task_service.dart';
import 'TaskDetails.dart';

const _primary = Color(0xFFB7873B);

class TasksListPage extends StatefulWidget {
  const TasksListPage({super.key});

  @override
  State<TasksListPage> createState() => _TasksListPageState();
}

class _TasksListPageState extends State<TasksListPage> {
  List<TaskItem>? _tasks;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    _tasks = await TaskService.getAll();
    if (mounted) setState(() => _loading = false);
  }

  Color _statusColor(int? status) {
    switch (status) {
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

  IconData _statusIcon(int? status) {
    switch (status) {
      case 1:
        return Icons.fiber_new_rounded;
      case 2:
        return Icons.pending_actions_rounded;
      case 3:
        return Icons.check_circle_rounded;
      default:
        return Icons.help_outline_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Tasks')),
      body: _loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: _load,
              color: _primary,
              child: _tasks == null || _tasks!.isEmpty
                  ? const _EmptyTasks()
                  : ListView.separated(
                      padding: const EdgeInsets.all(16),
                      itemCount: _tasks!.length,
                      separatorBuilder: (_, __) => const SizedBox(height: 10),
                      itemBuilder: (context, i) {
                        final task = _tasks![i];
                        return _TaskCard(
                          task: task,
                          statusColor: _statusColor(task.status),
                          statusIcon: _statusIcon(task.status),
                          onTap: () async {
                            await Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => TaskDetailsPage(task: task),
                              ),
                            );
                            _load();
                          },
                        );
                      },
                    ),
            ),
    );
  }
}

// ─── Task Card ────────────────────────────────────────────────────────────────

class _TaskCard extends StatelessWidget {
  final TaskItem task;
  final Color statusColor;
  final IconData statusIcon;
  final VoidCallback onTap;

  const _TaskCard({
    required this.task,
    required this.statusColor,
    required this.statusIcon,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
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
              child: Icon(statusIcon, color: statusColor, size: 20),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    task.title,
                    style: const TextStyle(
                        fontWeight: FontWeight.w600, fontSize: 14),
                  ),
                  const SizedBox(height: 3),
                  if (task.assignedToName != null)
                    Text(
                      'Assigned to: ${task.assignedToName}',
                      style: TextStyle(
                          fontSize: 11,
                          color: Theme.of(context)
                              .colorScheme
                              .onSurface
                              .withValues(alpha: 0.45)),
                    ),
                  const SizedBox(height: 5),
                  _StatusChip(label: task.statusLabel, color: statusColor),
                ],
              ),
            ),
            Icon(Icons.chevron_right_rounded,
                color: Theme.of(context)
                    .colorScheme
                    .onSurface
                    .withValues(alpha: 0.26)),
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

class _EmptyTasks extends StatelessWidget {
  const _EmptyTasks();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.task_alt_rounded,
                size: 56, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 16),
            Text('No tasks assigned',
                style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: onSurface.withValues(alpha: 0.38))),
            const SizedBox(height: 6),
            Text('Your task queue is clear.',
                style: TextStyle(
                    fontSize: 12, color: onSurface.withValues(alpha: 0.26))),
          ],
        ),
      ),
    );
  }
}
