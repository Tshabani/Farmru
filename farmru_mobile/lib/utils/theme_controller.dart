import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'UserSettings.dart';

class ThemeController extends GetxController {
  final _isDark = false.obs;

  bool get isDark => _isDark.value;
  ThemeMode get themeMode => _isDark.value ? ThemeMode.dark : ThemeMode.light;

  @override
  void onInit() {
    super.onInit();
    _isDark.value = UserSettings.getDarkMode();
  }

  Future<void> toggle(bool value) async {
    _isDark.value = value;
    await UserSettings.setDarkMode(value);
    Get.changeThemeMode(themeMode);
  }
}
