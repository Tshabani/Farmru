import '../models/crop_models.dart';
import '../utils/base_client.dart';

class CropSeasonService {
  static Future<List<CropSeasonResult>> GetByField(String fieldId) async {
    var response = await BaseClient().get(
        'api/services/app/CropSeason/GetByField?FieldId=$fieldId&SkipCount=0&MaxResultCount=50');
    if (response == null) return [];
    return cropSeasonListResponseFromJson(response).items;
  }

  static Future<CropSeasonDetailResult?> GetDetail(String cropSeasonId) async {
    var response = await BaseClient()
        .get('api/services/app/CropSeason/GetDetail?Id=$cropSeasonId');
    if (response == null) return null;
    return cropSeasonDetailResponseFromJson(response).result;
  }
}
