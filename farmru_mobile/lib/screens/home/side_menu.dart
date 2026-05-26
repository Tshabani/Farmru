import 'package:flutter/material.dart';
import '../../models/UserModel.dart';
import '../../services/user_services.dart';
import '../../utils/UserSettings.dart';
import '../auth/login_screen.dart';
import '../alerts/alerts_screen.dart';
import '../monitoring/monitoring_screen.dart';
import '../gis/operational_map_screen.dart';
import '../Incidents/IncidentsList.dart';
import '../Task/TaskList.dart';
import '../account/profile_screen.dart';
import '../account/policies_screen.dart';
import '../account/settings_screen.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class SideMenu extends StatefulWidget {
  const SideMenu({super.key});

  @override
  State<SideMenu> createState() => _SideMenuState();
}

class _SideMenuState extends State<SideMenu> {
  void _navigate(Widget screen) {
    Navigator.pop(context);
    Navigator.push(context, MaterialPageRoute(builder: (_) => screen));
  }

  void _logOut() async {
    UserSettings.setIsLoggedIn(false);
    UserSettings.removeToken();
    UserSettings.setCurrentUser('');
    if (!mounted) return;
    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (_) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    return Drawer(
      backgroundColor: surface,
      child: Column(
        children: [
          _DrawerHeader(),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.symmetric(vertical: 8),
              children: [
                const _SectionLabel('Field Operations'),
                _MenuItem(
                  icon: Icons.map_rounded,
                  label: 'Operational Map',
                  onTap: () => _navigate(const OperationalMapScreen()),
                ),
                _MenuItem(
                  icon: Icons.monitor_heart_rounded,
                  label: 'Monitoring',
                  onTap: () => _navigate(const MonitoringScreen()),
                ),
                _MenuItem(
                  icon: Icons.notifications_active_rounded,
                  label: 'Alerts',
                  onTap: () => _navigate(const AlertsScreen()),
                ),
                _MenuItem(
                  icon: Icons.report_problem_rounded,
                  label: 'Incidents',
                  onTap: () => _navigate(const IncidentListPage()),
                ),
                _MenuItem(
                  icon: Icons.task_alt_rounded,
                  label: 'Tasks',
                  onTap: () => _navigate(const TasksListPage()),
                ),
                const SizedBox(height: 8),
                const Divider(indent: 16, endIndent: 16),
                const SizedBox(height: 8),
                const _SectionLabel('Account'),
                _MenuItem(
                  icon: Icons.person_rounded,
                  label: 'Profile',
                  onTap: () => _navigate(const ProfileScreen()),
                ),
                _MenuItem(
                  icon: Icons.description_rounded,
                  label: 'Policies',
                  onTap: () => _navigate(const PoliciesScreen()),
                ),
                _MenuItem(
                  icon: Icons.settings_rounded,
                  label: 'Settings',
                  onTap: () => _navigate(const SettingsScreen()),
                ),
              ],
            ),
          ),
          const Divider(height: 1),
          _MenuItem(
            icon: Icons.logout_rounded,
            label: 'Logout',
            iconColor: Colors.red.shade400,
            labelColor: Colors.red.shade400,
            onTap: _logOut,
          ),
          const SizedBox(height: 12),
        ],
      ),
    );
  }
}

// ─── Drawer Header ────────────────────────────────────────────────────────────

class _DrawerHeader extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FutureBuilder<User?>(
      future: UserService.getUser(),
      builder: (context, snapshot) {
        final name = snapshot.data?.name ?? '';
        final email = snapshot.data?.emailAddress ?? '';
        return Container(
          width: double.infinity,
          decoration: const BoxDecoration(
            image: DecorationImage(
              image: NetworkImage(
                  'https://thumbs.dreamstime.com/b/green-valley-10835224.jpg'),
              fit: BoxFit.cover,
            ),
          ),
          child: Container(
            padding: EdgeInsets.fromLTRB(
                20, MediaQuery.of(context).padding.top + 24, 20, 24),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  Colors.black.withValues(alpha: 0.65),
                  Colors.black.withValues(alpha: 0.2),
                ],
                begin: Alignment.bottomCenter,
                end: Alignment.topCenter,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 52,
                  height: 52,
                  decoration: BoxDecoration(
                    color: _primary,
                    shape: BoxShape.circle,
                    border: Border.all(color: Colors.white, width: 2),
                  ),
                  child: Center(
                    child: Text(
                      name.isNotEmpty ? name[0].toUpperCase() : 'U',
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  name,
                  style: const TextStyle(
                      color: Colors.white,
                      fontSize: 16,
                      fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 2),
                Text(
                  email,
                  style: const TextStyle(color: Colors.white70, fontSize: 12),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

// ─── Section Label ────────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Padding(
        padding: const EdgeInsets.fromLTRB(20, 8, 20, 4),
        child: Text(
          text.toUpperCase(),
          style: TextStyle(
            fontSize: 10,
            fontWeight: FontWeight.w700,
            color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.38),
            letterSpacing: 1.2,
          ),
        ),
      );
}

// ─── Menu Item ────────────────────────────────────────────────────────────────

class _MenuItem extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;
  final Color? iconColor;
  final Color? labelColor;

  const _MenuItem({
    required this.icon,
    required this.label,
    required this.onTap,
    this.iconColor,
    this.labelColor,
  });

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    final ic = iconColor ?? _primary;
    final lc = labelColor ?? onSurface.withValues(alpha: 0.87);
    final isRed = iconColor == Colors.red.shade400;

    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 2),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 11),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(12),
          ),
          child: Row(
            children: [
              Container(
                width: 36,
                height: 36,
                decoration: BoxDecoration(
                  color: isRed
                      ? Colors.red.withValues(alpha: 0.08)
                      : _primaryLight,
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(icon, color: ic, size: 18),
              ),
              const SizedBox(width: 14),
              Text(
                label,
                style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w500,
                    color: lc),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
