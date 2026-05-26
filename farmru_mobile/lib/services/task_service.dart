import 'dart:convert';
import '../models/task_models.dart';
import '../utils/base_client.dart';

class TaskService {
  static const _base = 'api/services/app/TaskManagement';

  static Future<List<TaskItem>?> getAll() async {
    final response = await BaseClient().get('$_base/GetAll');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    final items = decoded['result']?['items'] as List<dynamic>? ?? [];
    return items.map((e) => TaskItem.fromJson(e)).toList();
  }

  static Future<TaskItem?> get(String id) async {
    final response = await BaseClient().get('$_base/Get?Id=$id');
    if (response == null) return null;
    final decoded = jsonDecode(response);
    return TaskItem.fromJson(decoded['result']);
  }

  static Future<bool> updateStatus(String id, int status) async {
    final current = await get(id);
    if (current == null) return false;
    final body = {
      'id': id,
      'title': current.title,
      'description': current.description,
      'status': status,
    };
    final response = await BaseClient().put('$_base/Update', body);
    return response != null;
  }
}
