// To parse this JSON data, do
//
//     final entityWithDisplayNameDto = entityWithDisplayNameDtoFromJson(jsonString);

// ignore_for_file: file_names

import 'dart:convert';

EntityWithDisplayNameDto entityWithDisplayNameDtoFromJson(String str) =>
    EntityWithDisplayNameDto.fromJson(json.decode(str));

String entityWithDisplayNameDtoToJson(EntityWithDisplayNameDto data) =>
    json.encode(data.toJson());

class EntityWithDisplayNameDto {
  EntityWithDisplayNameDto({
    this.displayName,
    this.id,
  });

  String? displayName;
  String? id;

  factory EntityWithDisplayNameDto.fromJson(Map<String, dynamic> json) {
    dynamic idValue = json["id"];
    String? idString;
    if (idValue is int) {
      idString = idValue.toString();
    } else {
      idString = idValue;
    }

    return EntityWithDisplayNameDto(
      displayName: json["displayText"],
      id: idString,
    );
  }

  Map<String, dynamic> toJson() => {
        "displayText": displayName,
        "id": id,
      };
}
