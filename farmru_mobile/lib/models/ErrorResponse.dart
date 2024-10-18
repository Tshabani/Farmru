// ignore_for_file: file_names
// To parse this JSON data, do
//
//     final errorResponse = errorResponseFromJson(jsonString);

import 'dart:convert';

ErrorResponse errorResponseFromJson(String str) =>
    ErrorResponse.fromJson(json.decode(str));

String errorResponseToJson(ErrorResponse data) => json.encode(data.toJson());

class ErrorResponse {
  ErrorResponse({
    this.error,
    this.unAuthorizedRequest,
    this.abp,
  });

  Error? error;
  bool? unAuthorizedRequest;
  bool? abp;

  factory ErrorResponse.fromJson(Map<String, dynamic> json) => ErrorResponse(
        error: json["error"] == null ? null : Error.fromJson(json["error"]),
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "error": error?.toJson(),
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}

class Error {
  Error({
    this.code,
    this.message,
    this.details,
    this.validationErrors,
  });

  int? code;
  String? message;
  dynamic details;
  dynamic validationErrors;

  factory Error.fromJson(Map<String, dynamic> json) => Error(
        code: json["code"],
        message: json["message"],
        details: json["details"],
        validationErrors: json["validationErrors"],
      );

  Map<String, dynamic> toJson() => {
        "code": code,
        "message": message,
        "details": details,
        "validationErrors": validationErrors,
      };
}
