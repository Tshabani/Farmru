// ignore_for_file: file_names

import 'package:shared_preferences/shared_preferences.dart';

class UserSettings {
  static late SharedPreferences _sharedPreferences;
  static const _keyToken = 'token';
  static const _isLoggedIn = 'isLoggedIn';
  static const _showIntroWizard = 'intro';
  static const _backendUrl = 'backendUrl';
  static const _user = 'user';
  static const _logSheet = 'user';

  static Future init() async {
    _sharedPreferences = await SharedPreferences.getInstance();
  }

  static Future setToken(String token) async {
    await _sharedPreferences.setString(_keyToken, token);
  }

  static Future setIsLoggedIn(bool isLoggedIn) async =>
      await _sharedPreferences.setBool(_isLoggedIn, isLoggedIn);

  static Future setShowWizard(bool show) async =>
      await _sharedPreferences.setBool(_showIntroWizard, show);

  static Future setBackendUrl(String backendUrl) async =>
      await _sharedPreferences.setString(_backendUrl, backendUrl);

  static Future setCurrentUser(String user) async =>
      await _sharedPreferences.setString(_user, user);

  // static Future setLogSheet(String logSheet) async =>
  //     await _sharedPreferences.setString(_logSheet, logSheet);

  static String getLogSheet() => _sharedPreferences.getString(_logSheet) ?? "";

  static String getToken() => _sharedPreferences.getString(_keyToken) ?? "";

  static Future removeToken() => _sharedPreferences.remove(_keyToken);

  static String getBackendUrl() =>
      _sharedPreferences.getString(_backendUrl) ?? "";

  static String getCurrentUser() => _sharedPreferences.getString(_user) ?? "";

  static bool getIsLoggedIn() =>
      _sharedPreferences.getBool(_isLoggedIn) ?? false;

  static bool getShowIntroWizzard() =>
      _sharedPreferences.getBool(_showIntroWizard) ?? true;
}
