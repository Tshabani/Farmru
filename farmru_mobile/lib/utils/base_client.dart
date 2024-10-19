// ignore_for_file: unused_field

import 'dart:convert';
import 'dart:io';
import 'package:get/get.dart';
import '../models/ErrorResponse.dart';
import '../screens/auth/login_screen.dart';
import '../screens/components/CustomToast.dart'; 
import 'UserSettings.dart';
import 'package:http/http.dart' as http;

class BaseClient {
  static String baseUrl = UserSettings.getBackendUrl();
  static String token = UserSettings.getToken();
  final client = http.Client();
  final _headers = {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json'
  };

  Future<dynamic> _handleResponse(http.Response response) async {
    final statusCode = response.statusCode;
    if (statusCode == HttpStatus.unauthorized) {
      UserSettings.setIsLoggedIn(false);
      UserSettings.removeToken();
      UserSettings.setCurrentUser('');
      CustomToast.errorShortToast('Session expired. Please login again.');

      Get.to(const LoginScreen());
      Get.back();
      Get.off(const LoginScreen());
    }

    final responseBody = response.body;
    if (statusCode == HttpStatus.notFound) {
      CustomToast.errorShortToast(
          'Oops! Something went wrong. Please try again later.');
      return null;
    } else if (statusCode < 200 || statusCode >= 300) {
      var error = errorResponseFromJson(responseBody);
      var errorMsg = error.error?.message;

      CustomToast.errorShortToast(
          'Request failed with status code $statusCode:  $errorMsg');
    }
    return responseBody;
  }

  Future<dynamic> _executeRequest(String method, String api,
      {dynamic payload}) async {
    try {
      final url = Uri.parse(baseUrl + api);
      final token2 = UserSettings.getToken();

      final headers = {
        'Content-Type': 'application/json',
        if (token2.isNotEmpty) 'Authorization': 'Bearer $token2',
      }..removeWhere((key, value) => value.isEmpty);

      final request = http.Request(method, url)
        ..headers.addAll(headers)
        ..body = (payload != null ? json.encode(payload) : "");

      final response =
          await client.send(request).then(http.Response.fromStream);
      return await _handleResponse(response);
    } catch (e) {
      CustomToast.errorShortToast('Failed to $method $api: $e');
      return null;
    }
  }

  Future<dynamic> get(String api) async {
    return _executeRequest('GET', api);
  }

  Future<dynamic> post(String api, dynamic payload) async {
    return _executeRequest('POST', api, payload: payload);
  }

  Future<dynamic> put(String api, dynamic payload) async {
    return _executeRequest('PUT', api, payload: payload);
  }

  Future<dynamic> delete(String api) async {
    return _executeRequest('DELETE', api);
  }
}
