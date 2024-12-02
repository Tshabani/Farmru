// To parse this JSON data, do
//
//     final tenantResponse = tenantResponseFromJson(jsonString);

import 'dart:convert';

TenantResponse tenantResponseFromJson(String str) =>
    TenantResponse.fromJson(json.decode(str));

String tenantResponseToJson(TenantResponse data) => json.encode(data.toJson());

class TenantResponse {
  Result result;
  dynamic targetUrl;
  bool success;
  dynamic error;
  bool unAuthorizedRequest;
  bool abp;

  TenantResponse({
    required this.result,
    required this.targetUrl,
    required this.success,
    required this.error,
    required this.unAuthorizedRequest,
    required this.abp,
  });

  factory TenantResponse.fromJson(Map<String, dynamic> json) => TenantResponse(
        result: Result.fromJson(json["result"]),
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

class Result {
  int totalCount;
  List<Tenant> items;

  Result({
    required this.totalCount,
    required this.items,
  });

  factory Result.fromJson(Map<String, dynamic> json) => Result(
        totalCount: json["totalCount"],
        items: List<Tenant>.from(json["items"].map((x) => Tenant.fromJson(x))),
      );

  Map<String, dynamic> toJson() => {
        "totalCount": totalCount,
        "items": List<dynamic>.from(items.map((x) => x.toJson())),
      };
}

class Tenant {
  String tenancyName;
  String name;
  bool isActive;
  int id;

  Tenant({
    required this.tenancyName,
    required this.name,
    required this.isActive,
    required this.id,
  });

  factory Tenant.fromJson(Map<String, dynamic> json) => Tenant(
        tenancyName: json["tenancyName"],
        name: json["name"],
        isActive: json["isActive"],
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "tenancyName": tenancyName,
        "name": name,
        "isActive": isActive,
        "id": id,
      };
}
