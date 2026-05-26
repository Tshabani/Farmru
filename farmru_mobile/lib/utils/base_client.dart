import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:get/get.dart';
import '../screens/auth/login_screen.dart';
import '../screens/components/CustomToast.dart';
import 'UserSettings.dart';
import 'package:http/http.dart' as http;

class BaseClient {
  static String baseUrl = UserSettings.getBackendUrl();
  final _client = http.Client();

  // ─── Response handler ──────────────────────────────────────────────────────

  Future<String?> _handleResponse(http.Response response) async {
    final status = response.statusCode;

    if (status == HttpStatus.unauthorized) {
      UserSettings.setIsLoggedIn(false);
      UserSettings.removeToken();
      UserSettings.setCurrentUser('');
      CustomToast.errorShortToast('Session expired. Please log in again.');
      Get.offAll(() => const LoginScreen());
      return null;
    }

    if (status == HttpStatus.notFound) {
      // 404 on list endpoints means empty data — show nothing.
      return null;
    }

    if (status >= 200 && status < 300) {
      return response.body;
    }

    // ── All other error codes: extract the best available message ──────────
    final message = _extractErrorMessage(response.body, status);
    CustomToast.errorShortToast(message);
    return null;
  }

  /// Extracts a human-readable error message from an ABP error response body.
  String _extractErrorMessage(String body, int statusCode) {
    try {
      final decoded = jsonDecode(body);
      if (decoded is Map<String, dynamic>) {
        final error = decoded['error'];
        if (error is Map<String, dynamic>) {
          // Validation errors take priority — list each field's message.
          final validations = error['validationErrors'];
          if (validations is List && validations.isNotEmpty) {
            final msgs = validations
                .map((v) => v['message']?.toString() ?? '')
                .where((m) => m.isNotEmpty)
                .toList();
            if (msgs.isNotEmpty) return msgs.join('\n');
          }

          // details is usually more specific than message in ABP.
          final details = error['details']?.toString();
          if (details != null && details.isNotEmpty) return details;

          final message = error['message']?.toString();
          if (message != null && message.isNotEmpty) return message;
        }
      }
    } catch (_) {}
    return 'Request failed (HTTP $statusCode). Please try again.';
  }

  // ─── Request executor ──────────────────────────────────────────────────────

  Future<String?> _execute(String method, String api,
      {dynamic payload}) async {
    try {
      final url = Uri.parse('${UserSettings.getBackendUrl()}$api');
      final token = UserSettings.getToken();

      final headers = <String, String>{
        'Content-Type': 'application/json',
        if (token.isNotEmpty) 'Authorization': 'Bearer $token',
      };

      final request = http.Request(method, url)
        ..headers.addAll(headers)
        ..body = payload != null ? json.encode(payload) : '';

      final response = await _client
          .send(request)
          .timeout(const Duration(seconds: 30))
          .then(http.Response.fromStream);

      return await _handleResponse(response);
    } on SocketException {
      CustomToast.errorShortToast(
          'No internet connection. Please check your network.');
    } on TimeoutException {
      CustomToast.errorShortToast(
          'The request timed out. Please try again.');
    } on HandshakeException {
      CustomToast.errorShortToast(
          'Secure connection failed. Check your network or try again later.');
    } on FormatException {
      CustomToast.errorShortToast(
          'Unexpected response from the server. Please try again.');
    } catch (e) {
      CustomToast.errorShortToast('Something went wrong: ${e.runtimeType}');
    }
    return null;
  }

  // ─── Public methods ────────────────────────────────────────────────────────

  Future<String?> get(String api) => _execute('GET', api);
  Future<String?> post(String api, dynamic payload) =>
      _execute('POST', api, payload: payload);
  Future<String?> put(String api, dynamic payload) =>
      _execute('PUT', api, payload: payload);
  Future<String?> delete(String api) => _execute('DELETE', api);
}
