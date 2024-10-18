// To parse this JSON data, do
//
//     final autoCompleteItem = autoCompleteItemFromJson(jsonString);

// ignore_for_file: file_names

import 'dart:convert';

import 'EntityWithDisplayNameDto.dart';

AutoCompleteItem autoCompleteItemFromJson(String str) =>
    AutoCompleteItem.fromJson(json.decode(str));

String autoCompleteItemToJson(AutoCompleteItem data) =>
    json.encode(data.toJson());

class AutoCompleteItem {
  AutoCompleteItem({
    this.result,
    this.targetUrl,
    this.success,
    this.error,
    this.unAuthorizedRequest,
    this.abp,
  });

  List<EntityWithDisplayNameDto>? result;
  dynamic targetUrl;
  bool? success;
  dynamic error;
  bool? unAuthorizedRequest;
  bool? abp;

  factory AutoCompleteItem.fromJson(Map<String, dynamic> json) =>
      AutoCompleteItem(
        result: json["result"] == null
            ? []
            : List<EntityWithDisplayNameDto>.from(json["result"]!
                .map((x) => EntityWithDisplayNameDto.fromJson(x))),
        targetUrl: json["targetUrl"],
        success: json["success"],
        error: json["error"],
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "result": result == null
            ? []
            : List<dynamic>.from(result!.map((x) => x.toJson())),
        "targetUrl": targetUrl,
        "success": success,
        "error": error,
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}
