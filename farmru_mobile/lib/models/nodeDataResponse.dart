// To parse this JSON data, do
//
//     final nodeDataResponse = nodeDataResponseFromJson(jsonString);

import 'dart:convert';

NodeDataResponse nodeDataResponseFromJson(String str) =>
    NodeDataResponse.fromJson(json.decode(str));

String nodeDataResponseToJson(NodeDataResponse data) =>
    json.encode(data.toJson());

class NodeDataResponse {
  List<NodeData> result;
  dynamic targetUrl;
  bool success;
  dynamic error;
  bool unAuthorizedRequest;
  bool abp;

  NodeDataResponse({
    required this.result,
    required this.targetUrl,
    required this.success,
    required this.error,
    required this.unAuthorizedRequest,
    required this.abp,
  });

  factory NodeDataResponse.fromJson(Map<String, dynamic> json) =>
      NodeDataResponse(
        result: List<NodeData>.from(
            json["result"].map((x) => NodeData.fromJson(x))),
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

class NodeData {
  String soilTemperature;
  String soilPh;
  String moisture;
  String phosphorus;
  String potassium;
  String nitrogen;
  int latitude;
  int longitude;
  int solarPanelVoltage;
  int batteryVoltage;
  dynamic conductivity;
  DateTime loggingTime;
  DateTime creationTime;
  Node node;
  String id;

  NodeData({
    required this.soilTemperature,
    required this.soilPh,
    required this.moisture,
    required this.phosphorus,
    required this.potassium,
    required this.nitrogen,
    required this.latitude,
    required this.longitude,
    required this.solarPanelVoltage,
    required this.batteryVoltage,
    required this.conductivity,
    required this.loggingTime,
    required this.creationTime,
    required this.node,
    required this.id,
  });

  factory NodeData.fromJson(Map<String, dynamic> json) => NodeData(
        soilTemperature: json["soilTemperature"],
        soilPh: json["soilPH"],
        moisture: json["moisture"],
        phosphorus: json["phosphorus"],
        potassium: json["potassium"],
        nitrogen: json["nitrogen"],
        latitude: json["latitude"],
        longitude: json["longitude"],
        solarPanelVoltage: json["solarPanelVoltage"],
        batteryVoltage: json["batteryVoltage"],
        conductivity: json["conductivity"],
        loggingTime: DateTime.parse(json["loggingTime"]),
        creationTime: DateTime.parse(json["creationTime"]),
        node: Node.fromJson(json["node"]),
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "soilTemperature": soilTemperature,
        "soilPH": soilPh,
        "moisture": moisture,
        "phosphorus": phosphorus,
        "potassium": potassium,
        "nitrogen": nitrogen,
        "latitude": latitude,
        "longitude": longitude,
        "solarPanelVoltage": solarPanelVoltage,
        "batteryVoltage": batteryVoltage,
        "conductivity": conductivity,
        "loggingTime": loggingTime.toIso8601String(),
        "creationTime": creationTime.toIso8601String(),
        "node": node.toJson(),
        "id": id,
      };
}

class Node {
  String displayText;
  String id;

  Node({
    required this.displayText,
    required this.id,
  });

  factory Node.fromJson(Map<String, dynamic> json) => Node(
        displayText: json["displayText"],
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "displayText": displayText,
        "id": id,
      };
}
