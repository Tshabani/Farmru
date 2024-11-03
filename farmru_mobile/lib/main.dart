import 'package:flutter/material.dart';
import 'package:get/get_navigation/src/root/get_material_app.dart';
import 'package:local_session_timeout/local_session_timeout.dart';

import 'screens/auth/login_screen.dart';
import 'utils/UserSettings.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await UserSettings.init();
  await UserSettings.setBackendUrl("https://farmruapi.technobrainent.co.za/");
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  final _navigatorKey = GlobalKey<NavigatorState>();
  NavigatorState get _navigator => _navigatorKey.currentState!;

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    final sessionConfig = SessionConfig(
      invalidateSessionForAppLostFocus: const Duration(minutes: 30),
      invalidateSessionForUserInactivity: const Duration(minutes: 30),
    );

    sessionConfig.stream.listen(
      (SessionTimeoutState timeoutEvent) {
        if (timeoutEvent == SessionTimeoutState.userInactivityTimeout) {
          UserSettings.setIsLoggedIn(false);
          UserSettings.removeToken();
          UserSettings.setCurrentUser('');
          _navigator.push(
            MaterialPageRoute(
              builder: (_) => const LoginScreen(),
            ),
          );
        } else if (timeoutEvent == SessionTimeoutState.appFocusTimeout) {
          UserSettings.setIsLoggedIn(false);
          UserSettings.removeToken();
          UserSettings.setCurrentUser('');
          _navigator.push(
            MaterialPageRoute(
              builder: (_) => const LoginScreen(),
            ),
          );
        }
      },
    );

    return SessionTimeoutManager(
      sessionConfig: sessionConfig,
      child: GetMaterialApp(
        debugShowCheckedModeBanner: false,
        navigatorKey: _navigatorKey,
        theme: ThemeData(
          primarySwatch: MaterialColor(
            0xFFB7873B,
            {
              50: const Color(0xFFB7873B).withOpacity(0.1),
              100: const Color(0xFFB7873B).withOpacity(0.2),
              200: const Color(0xFFB7873B).withOpacity(0.3),
              300: const Color(0xFFB7873B).withOpacity(0.4),
              400: const Color(0xFFB7873B).withOpacity(0.5),
              500: const Color(0xFFB7873B).withOpacity(0.6),
              600: const Color(0xFFB7873B).withOpacity(0.7),
              700: const Color(0xFFB7873B).withOpacity(0.8),
              800: const Color(0xFFB7873B).withOpacity(0.9),
              900: const Color(0xFFB7873B).withOpacity(1),
            },
          ),
        ),
        home: const LoginScreen(),
      ),
    );
  }
}
