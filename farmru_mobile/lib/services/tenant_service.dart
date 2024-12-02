// ignore_for_file: non_constant_identifier_names

import '../models/TenantResponse.dart';
import '../utils/base_client.dart';

class TenantService {
  static Future<List<String>?> GetAll() async {
    var response = await BaseClient().get('api/services/app/Tenant/GetAll');
    if (response == null) return null;
    var tenant = tenantResponseFromJson(response);

    return tenant.result.items.map((item) => item.name).toList();
  }
}
