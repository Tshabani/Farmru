class AlertItem {
  final String id;
  final int severity;
  final int alertType;
  final String title;
  final String? description;
  final bool isAcknowledged;
  final bool isResolved;
  final String? triggeredAt;
  final String? nodeDisplay;
  final String? facilityDisplay;

  AlertItem({
    required this.id,
    required this.severity,
    required this.alertType,
    required this.title,
    this.description,
    this.isAcknowledged = false,
    this.isResolved = false,
    this.triggeredAt,
    this.nodeDisplay,
    this.facilityDisplay,
  });

  factory AlertItem.fromJson(Map<String, dynamic> json) => AlertItem(
        id: json['id'] ?? '',
        severity: json['severity'] ?? 0,
        alertType: json['alertType'] ?? 0,
        title: json['title'] ?? '',
        description: json['description'],
        isAcknowledged: json['isAcknowledged'] ?? false,
        isResolved: json['isResolved'] ?? false,
        triggeredAt: json['triggeredAt'],
        nodeDisplay: json['node']?['displayText'],
        facilityDisplay: json['facility']?['displayText'],
      );

  String get severityLabel {
    const labels = ['Info', 'Warning', 'Critical'];
    if (severity < 0 || severity >= labels.length) return 'Unknown';
    return labels[severity];
  }
}
