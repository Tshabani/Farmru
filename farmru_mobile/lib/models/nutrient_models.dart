// Mirrors NutrientBalanceSnapshotDto from
// Farmru.IotMonitoring.Application/Services/Nutrients/Dto/NutrientDtos.cs.

import 'dart:convert';

NutrientBalanceResponse nutrientBalanceResponseFromJson(String str) =>
    NutrientBalanceResponse.fromJson(json.decode(str));

NutrientBalanceHistoryResponse nutrientBalanceHistoryResponseFromJson(String str) =>
    NutrientBalanceHistoryResponse.fromJson(json.decode(str));

class NutrientBalanceResponse {
  final NutrientBalanceSnapshotResult? result;

  NutrientBalanceResponse({required this.result});

  factory NutrientBalanceResponse.fromJson(Map<String, dynamic> json) =>
      NutrientBalanceResponse(
        result: json["result"] != null
            ? NutrientBalanceSnapshotResult.fromJson(json["result"])
            : null,
      );
}

class NutrientBalanceHistoryResponse {
  final List<NutrientBalanceSnapshotResult> result;

  NutrientBalanceHistoryResponse({required this.result});

  factory NutrientBalanceHistoryResponse.fromJson(Map<String, dynamic> json) =>
      NutrientBalanceHistoryResponse(
        result: json["result"] != null
            ? List<NutrientBalanceSnapshotResult>.from(
                json["result"].map((x) => NutrientBalanceSnapshotResult.fromJson(x)))
            : [],
      );
}

class NutrientBalanceSnapshotResult {
  final DateTime snapshotDate;
  final double sensedNitrogen;
  final double sensedPhosphorus;
  final double sensedPotassium;
  final double appliedNitrogenTrailing30d;
  final double appliedPhosphorusTrailing30d;
  final double appliedPotassiumTrailing30d;
  final int nitrogenStatus; // 0=Deficient 1=Adequate 2=Surplus
  final int phosphorusStatus;
  final int potassiumStatus;

  NutrientBalanceSnapshotResult({
    required this.snapshotDate,
    required this.sensedNitrogen,
    required this.sensedPhosphorus,
    required this.sensedPotassium,
    required this.appliedNitrogenTrailing30d,
    required this.appliedPhosphorusTrailing30d,
    required this.appliedPotassiumTrailing30d,
    required this.nitrogenStatus,
    required this.phosphorusStatus,
    required this.potassiumStatus,
  });

  factory NutrientBalanceSnapshotResult.fromJson(Map<String, dynamic> json) =>
      NutrientBalanceSnapshotResult(
        snapshotDate: DateTime.tryParse(json["snapshotDate"] ?? '') ?? DateTime.now(),
        sensedNitrogen: (json["sensedNitrogen"] ?? 0).toDouble(),
        sensedPhosphorus: (json["sensedPhosphorus"] ?? 0).toDouble(),
        sensedPotassium: (json["sensedPotassium"] ?? 0).toDouble(),
        appliedNitrogenTrailing30d: (json["appliedNitrogenTrailing30d"] ?? 0).toDouble(),
        appliedPhosphorusTrailing30d:
            (json["appliedPhosphorusTrailing30d"] ?? 0).toDouble(),
        appliedPotassiumTrailing30d:
            (json["appliedPotassiumTrailing30d"] ?? 0).toDouble(),
        nitrogenStatus: json["nitrogenStatus"] ?? 1,
        phosphorusStatus: json["phosphorusStatus"] ?? 1,
        potassiumStatus: json["potassiumStatus"] ?? 1,
      );
}

String nutrientStatusLabel(int status) {
  const labels = ['Deficient', 'Adequate', 'Surplus'];
  return (status >= 0 && status < labels.length) ? labels[status] : 'Unknown';
}
