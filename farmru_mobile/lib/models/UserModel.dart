// To parse this JSON data, do
//
//     final userModel = userModelFromJson(jsonString);

// ignore_for_file: file_names

import 'dart:convert';

UserModel userModelFromJson(String str) => UserModel.fromJson(json.decode(str));

String userModelToJson(UserModel data) => json.encode(data.toJson());

class UserModel {
  UserModel({
    this.result,
    this.targetUrl,
    this.success,
    this.error,
    this.unAuthorizedRequest,
    this.abp,
  });

  Result? result;
  dynamic targetUrl;
  bool? success;
  dynamic error;
  bool? unAuthorizedRequest;
  bool? abp;

  factory UserModel.fromJson(Map<String, dynamic> json) => UserModel(
        result: Result.fromJson(json["result"]),
        targetUrl: json["targetUrl"],
        success: json["success"],
        error: json["error"],
        unAuthorizedRequest: json["unAuthorizedRequest"],
        abp: json["__abp"],
      );

  Map<String, dynamic> toJson() => {
        "result": result?.toJson(),
        "targetUrl": targetUrl,
        "success": success,
        "error": error,
        "unAuthorizedRequest": unAuthorizedRequest,
        "__abp": abp,
      };
}

class Result {
  Result({
    this.application,
    required this.user,
    this.tenant,
  });

  Application? application;
  User user;
  dynamic tenant;

  factory Result.fromJson(Map<String, dynamic> json) => Result(
        application: Application.fromJson(json["application"]),
        user: User.fromJson(json["user"]),
        tenant: json["tenant"],
      );

  Map<String, dynamic> toJson() => {
        "application": application?.toJson(),
        "user": user.toJson(),
        "tenant": tenant,
      };
}

class Application {
  Application({
    this.version,
    this.releaseDate,
    this.features,
  });

  String? version;
  DateTime? releaseDate;
  Features? features;

  factory Application.fromJson(Map<String, dynamic> json) => Application(
        version: json["version"],
        releaseDate: DateTime.parse(json["releaseDate"]),
        features: Features.fromJson(json["features"]),
      );

  Map<String, dynamic> toJson() => {
        "version": version,
        "releaseDate": releaseDate?.toIso8601String(),
        "features": features?.toJson(),
      };
}

class Features {
  Features();

  factory Features.fromJson(Map<String, dynamic> json) => Features();

  Map<String, dynamic> toJson() => {};
}

class User {
  User({
    this.name,
    this.surname,
    this.userName,
    this.emailAddress,
    this.id,
  });

  String? name;
  String? surname;
  String? userName;
  String? emailAddress;
  int? id;

  factory User.fromJson(Map<String, dynamic> json) => User(
        name: json["name"],
        surname: json["surname"],
        userName: json["userName"],
        emailAddress: json["emailAddress"],
        id: json["id"],
      );

  Map<String, dynamic> toJson() => {
        "name": name,
        "surname": surname,
        "userName": userName,
        "emailAddress": emailAddress,
        "id": id,
      };
}
