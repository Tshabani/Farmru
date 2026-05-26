import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../utils/theme_controller.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  bool _alertNotifications = true;
  bool _incidentNotifications = true;
  bool _taskNotifications = true;

  ThemeController get _theme => Get.find<ThemeController>();

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final cardColor = isDark ? const Color(0xFF1E1E1E) : Colors.white;
    final labelColor = isDark ? Colors.white60 : Colors.black45;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Settings'),
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const _SectionLabel('Notifications'),
          const SizedBox(height: 8),
          _ToggleCard(
            icon: Icons.notifications_active_rounded,
            title: 'Alert Notifications',
            subtitle: 'Get notified when new alerts are triggered',
            value: _alertNotifications,
            cardColor: cardColor,
            labelColor: labelColor,
            onChanged: (v) => setState(() => _alertNotifications = v),
          ),
          const SizedBox(height: 10),
          _ToggleCard(
            icon: Icons.report_problem_rounded,
            title: 'Incident Notifications',
            subtitle: 'Get notified when incidents are assigned to you',
            value: _incidentNotifications,
            cardColor: cardColor,
            labelColor: labelColor,
            onChanged: (v) => setState(() => _incidentNotifications = v),
          ),
          const SizedBox(height: 10),
          _ToggleCard(
            icon: Icons.task_alt_rounded,
            title: 'Task Notifications',
            subtitle: 'Get notified when tasks are assigned to you',
            value: _taskNotifications,
            cardColor: cardColor,
            labelColor: labelColor,
            onChanged: (v) => setState(() => _taskNotifications = v),
          ),
          const SizedBox(height: 20),
          const _SectionLabel('Appearance'),
          const SizedBox(height: 8),
          Obx(() => _ToggleCard(
                icon: Icons.dark_mode_rounded,
                title: 'Dark Mode',
                subtitle: 'Switch to a dark colour scheme',
                value: _theme.isDark,
                cardColor: cardColor,
                labelColor: labelColor,
                onChanged: (v) => _theme.toggle(v),
              )),
          const SizedBox(height: 20),
          const _SectionLabel('About'),
          const SizedBox(height: 8),
          _InfoCard(
              icon: Icons.info_outline_rounded,
              title: 'App Version',
              value: '1.0.0',
              cardColor: cardColor,
              labelColor: labelColor),
          const SizedBox(height: 10),
          _InfoCard(
              icon: Icons.business_rounded,
              title: 'Provider',
              value: 'TechnoBrainent',
              cardColor: cardColor,
              labelColor: labelColor),
        ],
      ),
    );
  }
}

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Padding(
        padding: const EdgeInsets.only(left: 4, bottom: 2),
        child: Text(
          text.toUpperCase(),
          style: const TextStyle(
            fontSize: 10,
            fontWeight: FontWeight.w700,
            color: Colors.black38,
            letterSpacing: 1.2,
          ),
        ),
      );
}

class _ToggleCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final bool value;
  final Color cardColor;
  final Color labelColor;
  final ValueChanged<bool> onChanged;

  const _ToggleCard({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.value,
    required this.cardColor,
    required this.labelColor,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: cardColor,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: _primaryLight,
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(icon, color: _primary, size: 20),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title,
                    style: const TextStyle(
                        fontWeight: FontWeight.w600, fontSize: 14)),
                const SizedBox(height: 2),
                Text(subtitle, style: TextStyle(fontSize: 11, color: labelColor)),
              ],
            ),
          ),
          Switch(
            value: value,
            onChanged: onChanged,
            activeThumbColor: _primary,
            activeTrackColor: _primaryLight,
          ),
        ],
      ),
    );
  }
}

class _InfoCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;
  final Color cardColor;
  final Color labelColor;

  const _InfoCard({
    required this.icon,
    required this.title,
    required this.value,
    required this.cardColor,
    required this.labelColor,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      decoration: BoxDecoration(
        color: cardColor,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: _primaryLight,
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(icon, color: _primary, size: 20),
          ),
          const SizedBox(width: 14),
          Text(title, style: TextStyle(fontSize: 14, color: labelColor)),
          const Spacer(),
          Text(value,
              style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
        ],
      ),
    );
  }
}
