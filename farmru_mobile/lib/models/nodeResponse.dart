// To parse this JSON data, do
//
//     final nodeResponse = nodeResponseFromJson(jsonString);

import 'dart:convert';

NodeResponse nodeResponseFromJson(String str) =>
    NodeResponse.fromJson(json.decode(str));

String nodeResponseToJson(NodeResponse data) => json.encode(data.toJson());

class NodeResponse {
  List<NodeResult> result;
  dynamic targetUrl;
  bool success;
  dynamic error;
  bool unAuthorizedRequest;
  bool abp;

  NodeResponse({
    required this.result,
    required this.targetUrl,
    required this.success,
    required this.error,
    required this.unAuthorizedRequest,
    required this.abp,
  });

  factory NodeResponse.fromJson(Map<String, dynamic> json) => NodeResponse(
        result: List<NodeResult>.from(
            json["result"].map((x) => NodeResult.fromJson(x))),
        targetUrl: json["targetUrl"],
        success: json["success"],
        error: json["error"],
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "result": List<dynamic>.from(result.map((x) => x.toJson())),
        "targetUrl": targetUrl,
        "success": success,
        "error": error,
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}

class NodeResult {
  String serialNumber;
  String? displayName;
  Facility facility;
  String id;
  bool isActive;
  int deviceStatus;
  int healthStatus;
  String? lastSeenAt;
  double? batteryLevel;
  int? signalStrength;
  String? firmwareVersion;
  bool isOnline;

  NodeResult({
    required this.serialNumber,
    this.displayName,
    required this.facility,
    required this.id,
    this.isActive = true,
    this.deviceStatus = 0,
    this.healthStatus = 0,
    this.lastSeenAt,
    this.batteryLevel,
    this.signalStrength,
    this.firmwareVersion,
    this.isOnline = false,
  });

  factory NodeResult.fromJson(Map<String, dynamic> json) => NodeResult(
        serialNumber: json["serialNumber"] ?? '',
        displayName: json["displayName"],
        facility: json["facility"] != null
            ? Facility.fromJson(json["facility"])
            : Facility(displayText: '', id: ''),
        id: json["id"] ?? '',
        isActive: json["isActive"] ?? true,
        deviceStatus: json["deviceStatus"] ?? 0,
        healthStatus: json["healthStatus"] ?? 0,
        lastSeenAt: json["lastSeenAt"],
        batteryLevel: json["batteryLevel"]?.toDouble(),
        signalStrength: json["signalStrength"],
        firmwareVersion: json["firmwareVersion"],
        isOnline: json["isOnline"] ?? false,
      );

  Map<String, dynamic> toJson() => {
        "serialNumber": serialNumber,
        "displayName": displayName,
        "facility": facility.toJson(),
        "id": id,
        "isActive": isActive,
        "deviceStatus": deviceStatus,
        "healthStatus": healthStatus,
        "lastSeenAt": lastSeenAt,
        "batteryLevel": batteryLevel,
        "signalStrength": signalStrength,
        "firmwareVersion": firmwareVersion,
        "isOnline": isOnline,
      };

  String get healthLabel {
    const labels = ['Unknown', 'Healthy', 'Warning', 'Critical'];
    if (healthStatus < 0 || healthStatus >= labels.length) return 'Unknown';
    return labels[healthStatus];
  }
}

class Facility {
  String displayText;
  String id;

  Facility({
    required this.displayText,
    required this.id,
  });

  factory Facility.fromJson(Map<String, dynamic> json) => Facility(
        displayText: json["displayText"],
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "displayText": displayText,
        "id": id,
      };
}
