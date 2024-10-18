// ignore_for_file: unnecessary_new, body_might_complete_normally_nullable

import '../models/AutoCompleteItem.dart';
import '../models/EntityWithDisplayNameDto.dart';
import '../utils/UserSettings.dart';
import '../utils/base_client.dart';

class AutocompleteService {
  static String backendUrl = UserSettings.getBackendUrl();

  static Future<List<EntityWithDisplayNameDto>> usersAutoComplete(
      String term) async {
    return _autoCompleteRequest('UserAutoComplete', term);
  }

  static Future<List<EntityWithDisplayNameDto>> _autoCompleteRequest(
      String path, String term) async {
    var endpoint = 'api/services/app/AutoComplete/$path';
    var response = await _postAutocompleteRequest(endpoint, term);
    var result = autoCompleteItemFromJson(response).result;
    return _filterAutocompleteResults(result, term);
  }

  static Future<String> _postAutocompleteRequest(
      String endpoint, String term) async {
    var response = await BaseClient().post(endpoint, term).catchError((err) {});

    if (response == null) throw Exception();

    return response;
  }

  static List<T> _filterAutocompleteResults<T extends EntityWithDisplayNameDto>(
      List<T>? items, String term) {
    return items!.where((item) {
      final displayNameLower = item.displayName?.toLowerCase();
      final termLower = term.toLowerCase();
      return displayNameLower!.contains(termLower);
    }).toList();
  }
}
