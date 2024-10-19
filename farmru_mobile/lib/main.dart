import 'package:flutter/material.dart';
import 'package:get/get_navigation/src/root/get_material_app.dart';
import 'package:local_session_timeout/local_session_timeout.dart';

import 'screens/auth/login_screen.dart';
import 'utils/UserSettings.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await UserSettings.init();
  await UserSettings.setBackendUrl("http://farmruapi.technobrainent.co.za/");
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
          primarySwatch: Colors.deepOrange,
        ),
        home: const LoginScreen(),
      ),
    );
  }
}
