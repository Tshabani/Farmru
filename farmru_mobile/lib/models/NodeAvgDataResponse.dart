// To parse this JSON data, do
//
//     final nodeAvgDataResponse = nodeAvgDataResponseFromJson(jsonString);

import 'dart:convert';

NodeAvgDataResponse nodeAvgDataResponseFromJson(String str) =>
    NodeAvgDataResponse.fromJson(json.decode(str));

String nodeAvgDataResponseToJson(NodeAvgDataResponse data) =>
    json.encode(data.toJson());

class NodeAvgDataResponse {
  NodeAvgData result;
  dynamic targetUrl;
  bool success;
  dynamic error;
  bool unAuthorizedRequest;
  bool abp;

  NodeAvgDataResponse({
    required this.result,
    required this.targetUrl,
    required this.success,
    required this.error,
    required this.unAuthorizedRequest,
    required this.abp,
  });

  factory NodeAvgDataResponse.fromJson(Map<String, dynamic> json) =>
      NodeAvgDataResponse(
        result: NodeAvgData.fromJson(json["result"]),
        targetUrl: json["targetUrl"],
        success: json["success"],
        error: json["error"],
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "result": result.toJson(),
        "targetUrl": targetUrl,
        "success": success,
        "error": error,
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}

class NodeAvgData {
  double avgSoilTemperature;
  double avgSoilPh;
  double avgMoisture;
  double avgPhosphorus;
  double avgPotassium;
  double avgNitrogen;
  double avgSolarPanelVoltage;
  double avgBatteryVoltage;
  String id;
  LazyLoader lazyLoader;

  NodeAvgData({
    required this.avgSoilTemperature,
    required this.avgSoilPh,
    required this.avgMoisture,
    required this.avgPhosphorus,
    required this.avgPotassium,
    required this.avgNitrogen,
    required this.avgSolarPanelVoltage,
    required this.avgBatteryVoltage,
    required this.id,
    required this.lazyLoader,
  });

  factory NodeAvgData.fromJson(Map<String, dynamic> json) => NodeAvgData(
        avgSoilTemperature: json["avgSoilTemperature"]?.toDouble(),
        avgSoilPh: json["avgSoilPH"]?.toDouble(),
        avgMoisture: json["avgMoisture"]?.toDouble(),
        avgPhosphorus: json["avgPhosphorus"],
        avgPotassium: json["avgPotassium"],
        avgNitrogen: json["avgNitrogen"],
        avgSolarPanelVoltage: json["avgSolarPanelVoltage"],
        avgBatteryVoltage: json["avgBatteryVoltage"],
        id: json["id"],
        lazyLoader: LazyLoader.fromJson(json["lazyLoader"]),
      );

  Map<String, dynamic> toJson() => {
        "avgSoilTemperature": avgSoilTemperature,
        "avgSoilPH": avgSoilPh,
        "avgMoisture": avgMoisture,
        "avgPhosphorus": avgPhosphorus,
        "avgPotassium": avgPotassium,
        "avgNitrogen": avgNitrogen,
        "avgSolarPanelVoltage": avgSolarPanelVoltage,
        "avgBatteryVoltage": avgBatteryVoltage,
        "id": id,
        "lazyLoader": lazyLoader.toJson(),
      };
}

class LazyLoader {
  LazyLoader();

  factory LazyLoader.fromJson(Map<String, dynamic> json) => LazyLoader();

  Map<String, dynamic> toJson() => {};
}
