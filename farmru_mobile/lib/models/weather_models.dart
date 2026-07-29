// Mirrors WeatherObservationDto / WeatherForecastDto from
// Farmru.IotMonitoring.Application/Services/Weather/Dto/WeatherDtos.cs.
// Hand-written per the existing mobile convention (BaseClient + manual JSON
// parsing, see NodeService) — this codebase has no generated Dart client.

import 'dart:convert';

WeatherCurrentResponse weatherCurrentResponseFromJson(String str) =>
    WeatherCurrentResponse.fromJson(json.decode(str));

WeatherForecastListResponse weatherForecastListResponseFromJson(String str) =>
    WeatherForecastListResponse.fromJson(json.decode(str));

class WeatherCurrentResponse {
  final WeatherObservationResult? result;
  final bool success;

  WeatherCurrentResponse({required this.result, required this.success});

  factory WeatherCurrentResponse.fromJson(Map<String, dynamic> json) =>
      WeatherCurrentResponse(
        result: json["result"] != null
            ? WeatherObservationResult.fromJson(json["result"])
            : null,
        success: json["success"] ?? false,
      );
}

class WeatherObservationResult {
  final DateTime observedAt;
  final double temperatureCelsius;
  final double humidityPercent;
  final double? windSpeedKph;
  final double? precipitationMm;
  final double? uvIndex;
  final double? lightningProbabilityPercent;

  WeatherObservationResult({
    required this.observedAt,
    required this.temperatureCelsius,
    required this.humidityPercent,
    this.windSpeedKph,
    this.precipitationMm,
    this.uvIndex,
    this.lightningProbabilityPercent,
  });

  factory WeatherObservationResult.fromJson(Map<String, dynamic> json) =>
      WeatherObservationResult(
        observedAt: DateTime.tryParse(json["observedAt"] ?? '') ?? DateTime.now(),
        temperatureCelsius: (json["temperatureCelsius"] ?? 0).toDouble(),
        humidityPercent: (json["humidityPercent"] ?? 0).toDouble(),
        windSpeedKph: json["windSpeedKph"]?.toDouble(),
        precipitationMm: json["precipitationMm"]?.toDouble(),
        uvIndex: json["uvIndex"]?.toDouble(),
        lightningProbabilityPercent:
            json["lightningProbabilityPercent"]?.toDouble(),
      );
}

class WeatherForecastListResponse {
  final List<WeatherForecastResult> result;
  final bool success;

  WeatherForecastListResponse({required this.result, required this.success});

  factory WeatherForecastListResponse.fromJson(Map<String, dynamic> json) =>
      WeatherForecastListResponse(
        result: json["result"] != null
            ? List<WeatherForecastResult>.from(
                json["result"].map((x) => WeatherForecastResult.fromJson(x)))
            : [],
        success: json["success"] ?? false,
      );
}

class WeatherForecastResult {
  final DateTime forecastFor;
  final double tempMinCelsius;
  final double tempMaxCelsius;
  final int precipitationProbabilityPercent;
  final int frostRisk;
  final int heatStress;

  WeatherForecastResult({
    required this.forecastFor,
    required this.tempMinCelsius,
    required this.tempMaxCelsius,
    required this.precipitationProbabilityPercent,
    required this.frostRisk,
    required this.heatStress,
  });

  factory WeatherForecastResult.fromJson(Map<String, dynamic> json) =>
      WeatherForecastResult(
        forecastFor: DateTime.tryParse(json["forecastFor"] ?? '') ?? DateTime.now(),
        tempMinCelsius: (json["tempMinCelsius"] ?? 0).toDouble(),
        tempMaxCelsius: (json["tempMaxCelsius"] ?? 0).toDouble(),
        precipitationProbabilityPercent:
            json["precipitationProbabilityPercent"] ?? 0,
        frostRisk: json["frostRisk"] ?? 0,
        heatStress: json["heatStress"] ?? 0,
      );
}
