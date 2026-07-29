import '../models/nutrient_models.dart';
import '../utils/base_client.dart';

class NutrientService {
  static Future<NutrientBalanceSnapshotResult?> GetLatest(String fieldId) async {
    var response = await BaseClient()
        .get('api/services/app/NutrientBalance/GetLatest?Id=$fieldId');
    if (response == null) return null;
    return nutrientBalanceResponseFromJson(response).result;
  }

  static Future<List<NutrientBalanceSnapshotResult>> GetHistory(String fieldId) async {
    var response = await BaseClient()
        .get('api/services/app/NutrientBalance/GetHistory?FieldId=$fieldId');
    if (response == null) return [];
    return nutrientBalanceHistoryResponseFromJson(response).result;
  }
}
