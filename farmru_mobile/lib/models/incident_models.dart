class IncidentItem {
  final String id;
  final String title;
  final String? description;
  final int status;
  final int priority;
  final int slaStatus;
  final String createdDate;
  final String? responseDueAt;
  final String? resolutionDueAt;
  final String? assignedToName;
  final double? latitude;
  final double? longitude;

  IncidentItem({
    required this.id,
    required this.title,
    this.description,
    required this.status,
    required this.priority,
    required this.slaStatus,
    required this.createdDate,
    this.responseDueAt,
    this.resolutionDueAt,
    this.assignedToName,
    this.latitude,
    this.longitude,
  });

  factory IncidentItem.fromJson(Map<String, dynamic> json) => IncidentItem(
        id: json['id']?.toString() ?? '',
        title: json['title'] ?? '',
        description: json['description'],
        status: json['status'] ?? 0,
        priority: json['priority'] ?? 0,
        slaStatus: json['slaStatus'] ?? 0,
        createdDate: json['createdDate'] ?? '',
        responseDueAt: json['responseDueAt'],
        resolutionDueAt: json['resolutionDueAt'],
        assignedToName: json['assignedTo']?['displayText'],
        latitude: (json['latitude'] as num?)?.toDouble(),
        longitude: (json['longitude'] as num?)?.toDouble(),
      );
}

class IncidentTimelineItem {
  final String title;
  final String? description;
  final String creationTime;

  IncidentTimelineItem({
    required this.title,
    this.description,
    required this.creationTime,
  });

  factory IncidentTimelineItem.fromJson(Map<String, dynamic> json) =>
      IncidentTimelineItem(
        title: json['title'] ?? '',
        description: json['description'],
        creationTime: json['creationTime'] ?? '',
      );
}

class IncidentDetail extends IncidentItem {
  final List<IncidentTimelineItem> timeline;

  IncidentDetail({
    required super.id,
    required super.title,
    super.description,
    required super.status,
    required super.priority,
    required super.slaStatus,
    required super.createdDate,
    super.responseDueAt,
    super.resolutionDueAt,
    super.assignedToName,
    super.latitude,
    super.longitude,
    required this.timeline,
  });

  factory IncidentDetail.fromJson(Map<String, dynamic> json) {
    final base = IncidentItem.fromJson(json);
    final timeline = (json['timeline'] as List<dynamic>? ?? [])
        .map((e) => IncidentTimelineItem.fromJson(e))
        .toList();
    return IncidentDetail(
      id: base.id,
      title: base.title,
      description: base.description,
      status: base.status,
      priority: base.priority,
      slaStatus: base.slaStatus,
      createdDate: base.createdDate,
      responseDueAt: base.responseDueAt,
      resolutionDueAt: base.resolutionDueAt,
      assignedToName: base.assignedToName,
      latitude: base.latitude,
      longitude: base.longitude,
      timeline: timeline,
    );
  }
}
