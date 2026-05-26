class TaskItem {
  final String id;
  final String title;
  final String? description;
  final int? status;
  final String? assignedToName;
  final String? assignedByName;

  TaskItem({
    required this.id,
    required this.title,
    this.description,
    this.status,
    this.assignedToName,
    this.assignedByName,
  });

  factory TaskItem.fromJson(Map<String, dynamic> json) => TaskItem(
        id: json['id']?.toString() ?? '',
        title: json['title'] ?? '',
        description: json['description'],
        status: json['status'],
        assignedToName: json['assignedTo']?['displayText'],
        assignedByName: json['assignedBy']?['displayText'],
      );

  Map<String, dynamic> toJson() => {
        'title': title,
        'description': description,
        'status': status,
      };

  String get statusLabel {
    switch (status) {
      case 1:
        return 'New';
      case 2:
        return 'In Progress';
      case 3:
        return 'Closed';
      default:
        return 'Unknown';
    }
  }
}
