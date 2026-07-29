import '../models/crop_models.dart';
import '../utils/base_client.dart';

class FieldService {
  static Future<List<FieldResult>> GetAll() async {
    var response = await BaseClient()
        .get('api/services/app/Field/GetAll?SkipCount=0&MaxResultCount=200');
    if (response == null) return [];
    return fieldListResponseFromJson(response).items;
  }
}
