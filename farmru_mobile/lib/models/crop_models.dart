// Mirrors FieldDto / CropSeasonDto / CropSeasonDetailDto from
// Farmru.IotMonitoring.Application/Services/Crops/Dto/CropDtos.cs.

import 'dart:convert';

FieldListResponse fieldListResponseFromJson(String str) =>
    FieldListResponse.fromJson(json.decode(str));

CropSeasonListResponse cropSeasonListResponseFromJson(String str) =>
    CropSeasonListResponse.fromJson(json.decode(str));

CropSeasonDetailResponse cropSeasonDetailResponseFromJson(String str) =>
    CropSeasonDetailResponse.fromJson(json.decode(str));

class DisplayRef {
  final String id;
  final String displayText;

  DisplayRef({required this.id, required this.displayText});

  factory DisplayRef.fromJson(Map<String, dynamic>? json) => DisplayRef(
        id: json?["id"] ?? '',
        displayText: json?["displayText"] ?? '',
      );
}

class FieldListResponse {
  final List<FieldResult> items;

  FieldListResponse({required this.items});

  factory FieldListResponse.fromJson(Map<String, dynamic> json) {
    final result = json["result"];
    final rawItems = result is Map<String, dynamic> ? result["items"] : result;
    return FieldListResponse(
      items: rawItems != null
          ? List<FieldResult>.from(rawItems.map((x) => FieldResult.fromJson(x)))
          : [],
    );
  }
}

class FieldResult {
  final String id;
  final String name;
  final DisplayRef facility;
  final double? areaHectares;
  final String? soilType;

  FieldResult({
    required this.id,
    required this.name,
    required this.facility,
    this.areaHectares,
    this.soilType,
  });

  factory FieldResult.fromJson(Map<String, dynamic> json) => FieldResult(
        id: json["id"] ?? '',
        name: json["name"] ?? '',
        facility: DisplayRef.fromJson(json["facility"]),
        areaHectares: json["areaHectares"]?.toDouble(),
        soilType: json["soilType"],
      );
}

class CropSeasonListResponse {
  final List<CropSeasonResult> items;

  CropSeasonListResponse({required this.items});

  factory CropSeasonListResponse.fromJson(Map<String, dynamic> json) {
    final result = json["result"];
    final rawItems = result is Map<String, dynamic> ? result["items"] : result;
    return CropSeasonListResponse(
      items: rawItems != null
          ? List<CropSeasonResult>.from(rawItems.map((x) => CropSeasonResult.fromJson(x)))
          : [],
    );
  }
}

class CropSeasonResult {
  final String id;
  final DisplayRef field;
  final DisplayRef cropType;
  final DateTime plantingDate;
  final DateTime expectedHarvestDate;
  final int status; // CropSeasonStatus: 0=Planned 1=Growing 2=Harvested 3=Closed

  CropSeasonResult({
    required this.id,
    required this.field,
    required this.cropType,
    required this.plantingDate,
    required this.expectedHarvestDate,
    required this.status,
  });

  factory CropSeasonResult.fromJson(Map<String, dynamic> json) => CropSeasonResult(
        id: json["id"] ?? '',
        field: DisplayRef.fromJson(json["field"]),
        cropType: DisplayRef.fromJson(json["cropType"]),
        plantingDate: DateTime.tryParse(json["plantingDate"] ?? '') ?? DateTime.now(),
        expectedHarvestDate:
            DateTime.tryParse(json["expectedHarvestDate"] ?? '') ?? DateTime.now(),
        status: json["status"] ?? 0,
      );

  String get statusLabel {
    const labels = ['Planned', 'Growing', 'Harvested', 'Closed'];
    return (status >= 0 && status < labels.length) ? labels[status] : 'Unknown';
  }
}

class CropSeasonDetailResponse {
  final CropSeasonDetailResult? result;

  CropSeasonDetailResponse({required this.result});

  factory CropSeasonDetailResponse.fromJson(Map<String, dynamic> json) =>
      CropSeasonDetailResponse(
        result: json["result"] != null
            ? CropSeasonDetailResult.fromJson(json["result"])
            : null,
      );
}

class CropSeasonDetailResult extends CropSeasonResult {
  final List<GrowthStageEventResult> stageEvents;
  final HarvestRecordResult? harvest;

  CropSeasonDetailResult({
    required super.id,
    required super.field,
    required super.cropType,
    required super.plantingDate,
    required super.expectedHarvestDate,
    required super.status,
    required this.stageEvents,
    this.harvest,
  });

  factory CropSeasonDetailResult.fromJson(Map<String, dynamic> json) =>
      CropSeasonDetailResult(
        id: json["id"] ?? '',
        field: DisplayRef.fromJson(json["field"]),
        cropType: DisplayRef.fromJson(json["cropType"]),
        plantingDate: DateTime.tryParse(json["plantingDate"] ?? '') ?? DateTime.now(),
        expectedHarvestDate:
            DateTime.tryParse(json["expectedHarvestDate"] ?? '') ?? DateTime.now(),
        status: json["status"] ?? 0,
        stageEvents: json["stageEvents"] != null
            ? List<GrowthStageEventResult>.from(
                json["stageEvents"].map((x) => GrowthStageEventResult.fromJson(x)))
            : [],
        harvest: json["harvest"] != null
            ? HarvestRecordResult.fromJson(json["harvest"])
            : null,
      );
}

class GrowthStageEventResult {
  final int stage; // 0=Planted 1=Germination 2=Vegetative 3=Flowering 4=Fruiting 5=Maturity 6=Harvested
  final DateTime observedDate;

  GrowthStageEventResult({required this.stage, required this.observedDate});

  factory GrowthStageEventResult.fromJson(Map<String, dynamic> json) =>
      GrowthStageEventResult(
        stage: json["stage"] ?? 0,
        observedDate: DateTime.tryParse(json["observedDate"] ?? '') ?? DateTime.now(),
      );

  String get stageLabel {
    const labels = [
      'Planted',
      'Germination',
      'Vegetative',
      'Flowering',
      'Fruiting',
      'Maturity',
      'Harvested'
    ];
    return (stage >= 0 && stage < labels.length) ? labels[stage] : 'Unknown';
  }
}

class HarvestRecordResult {
  final DateTime harvestDate;
  final double actualYieldKg;
  final String? qualityGrade;

  HarvestRecordResult({
    required this.harvestDate,
    required this.actualYieldKg,
    this.qualityGrade,
  });

  factory HarvestRecordResult.fromJson(Map<String, dynamic> json) => HarvestRecordResult(
        harvestDate: DateTime.tryParse(json["harvestDate"] ?? '') ?? DateTime.now(),
        actualYieldKg: (json["actualYieldKg"] ?? 0).toDouble(),
        qualityGrade: json["qualityGrade"],
      );
}
