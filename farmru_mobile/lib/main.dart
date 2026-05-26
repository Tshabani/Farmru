import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:local_session_timeout/local_session_timeout.dart';

import 'screens/auth/login_screen.dart';
import 'utils/UserSettings.dart';
import 'utils/theme_controller.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await UserSettings.init();
  await UserSettings.setBackendUrl("https://farmruapi.technobrainent.co.za/");
  Get.put(ThemeController());
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  final _navigatorKey = GlobalKey<NavigatorState>();
  NavigatorState get _navigator => _navigatorKey.currentState!;

  @override
  Widget build(BuildContext context) {
    final themeController = Get.find<ThemeController>();

    final sessionConfig = SessionConfig(
      invalidateSessionForAppLostFocus: const Duration(minutes: 30),
      invalidateSessionForUserInactivity: const Duration(minutes: 30),
    );

    sessionConfig.stream.listen((SessionTimeoutState timeoutEvent) {
      if (timeoutEvent == SessionTimeoutState.userInactivityTimeout ||
          timeoutEvent == SessionTimeoutState.appFocusTimeout) {
        UserSettings.setIsLoggedIn(false);
        UserSettings.removeToken();
        UserSettings.setCurrentUser('');
        _navigator.push(
            MaterialPageRoute(builder: (_) => const LoginScreen()));
      }
    });

    return SessionTimeoutManager(
      sessionConfig: sessionConfig,
      child: GetMaterialApp(
        debugShowCheckedModeBanner: false,
        navigatorKey: _navigatorKey,
        themeMode: themeController.themeMode,
        theme: _lightTheme,
        darkTheme: _darkTheme,
        home: const LoginScreen(),
      ),
    );
  }
}

// ─── Light Theme ──────────────────────────────────────────────────────────────

final _lightTheme = ThemeData(
  brightness: Brightness.light,
  primaryColor: _primary,
  scaffoldBackgroundColor: const Color(0xFFF4F6F8),
  colorScheme: const ColorScheme.light(
    primary: _primary,
    secondary: _primary,
    surface: Colors.white,
  ),
  appBarTheme: const AppBarTheme(
    backgroundColor: _primary,
    foregroundColor: Colors.white,
    iconTheme: IconThemeData(color: Colors.white),
    titleTextStyle: TextStyle(
        color: Colors.white, fontSize: 18, fontWeight: FontWeight.w600),
  ),
  cardColor: Colors.white,
  dividerColor: Colors.black12,
  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: Colors.grey.shade100,
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(12),
      borderSide: BorderSide.none,
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(12),
      borderSide: const BorderSide(color: _primary),
    ),
  ),
  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: _primary,
      foregroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
    ),
  ),
  switchTheme: SwitchThemeData(
    thumbColor: WidgetStateProperty.resolveWith(
        (s) => s.contains(WidgetState.selected) ? _primary : Colors.grey),
    trackColor: WidgetStateProperty.resolveWith(
        (s) => s.contains(WidgetState.selected) ? _primaryLight : Colors.grey.shade300),
  ),
);

// ─── Dark Theme ───────────────────────────────────────────────────────────────

final _darkTheme = ThemeData(
  brightness: Brightness.dark,
  primaryColor: _primary,
  scaffoldBackgroundColor: const Color(0xFF121212),
  colorScheme: const ColorScheme.dark(
    primary: _primary,
    secondary: _primary,
    surface: Color(0xFF1E1E1E),
  ),
  appBarTheme: const AppBarTheme(
    backgroundColor: Color(0xFF1A1A1A),
    foregroundColor: Colors.white,
    iconTheme: IconThemeData(color: Colors.white),
    titleTextStyle: TextStyle(
        color: Colors.white, fontSize: 18, fontWeight: FontWeight.w600),
  ),
  cardColor: const Color(0xFF1E1E1E),
  dividerColor: Colors.white12,
  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: const Color(0xFF2A2A2A),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(12),
      borderSide: BorderSide.none,
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(12),
      borderSide: const BorderSide(color: _primary),
    ),
    labelStyle: const TextStyle(color: Colors.white60),
    hintStyle: const TextStyle(color: Colors.white38),
  ),
  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: _primary,
      foregroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
    ),
  ),
  switchTheme: SwitchThemeData(
    thumbColor: WidgetStateProperty.resolveWith(
        (s) => s.contains(WidgetState.selected) ? _primary : Colors.grey),
    trackColor: WidgetStateProperty.resolveWith(
        (s) => s.contains(WidgetState.selected) ? _primaryLight : Colors.grey.shade700),
  ),
);
