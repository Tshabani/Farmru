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
  Facility facility;
  String id;

  NodeResult({
    required this.serialNumber,
    required this.facility,
    required this.id,
  });

  factory NodeResult.fromJson(Map<String, dynamic> json) => NodeResult(
        serialNumber: json["serialNumber"],
        facility: Facility.fromJson(json["facility"]),
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "serialNumber": serialNumber,
        "facility": facility.toJson(),
        "id": id,
      };
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
