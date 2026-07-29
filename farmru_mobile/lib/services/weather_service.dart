import '../models/weather_models.dart';
import '../utils/base_client.dart';

class WeatherService {
  static Future<WeatherObservationResult?> GetCurrent(String facilityId) async {
    var response = await BaseClient()
        .get('api/services/app/Weather/GetCurrent?Id=$facilityId');
    if (response == null) return null;
    return weatherCurrentResponseFromJson(response).result;
  }

  static Future<List<WeatherForecastResult>> GetForecast(String facilityId) async {
    var response = await BaseClient()
        .get('api/services/app/Weather/GetForecast?Id=$facilityId');
    if (response == null) return [];
    return weatherForecastListResponseFromJson(response).result;
  }
}
