// To parse this JSON data, do
//
//     final genericApiResponse = genericApiResponseFromJson(jsonString);

// ignore_for_file: file_names

import 'dart:convert';

GenericApiResponse genericApiResponseFromJson(String str) =>
    GenericApiResponse.fromJson(json.decode(str));

String genericApiResponseToJson(GenericApiResponse data) =>
    json.encode(data.toJson());

class GenericApiResponse {
  GenericApiResponse({
    this.targetUrl,
    required this.success,
    this.error,
    this.unAuthorizedRequest,
    this.abp,
  });

  dynamic targetUrl;
  bool success;
  dynamic error;
  bool? unAuthorizedRequest;
  bool? abp;

  factory GenericApiResponse.fromJson(Map<String, dynamic> json) =>
      GenericApiResponse(
        targetUrl: json["targetUrl"],
        success: json["success"],
        error: json["error"],
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "targetUrl": targetUrl,
        "success": success,
        "error": error,
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}
