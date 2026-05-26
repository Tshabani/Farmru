import 'package:flutter/material.dart';
import '../../models/UserModel.dart';
import '../../services/user_services.dart';
import 'change_password_screen.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  User? _user;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    _user = await UserService.getUser();
    if (mounted) setState(() => _loading = false);
  }

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final cardColor = isDark ? const Color(0xFF1E1E1E) : Colors.white;
    final subtitleColor = isDark ? Colors.white54 : Colors.black45;

    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: _loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                children: [
                  const SizedBox(height: 12),
                  _Avatar(name: _user?.name ?? ''),
                  const SizedBox(height: 20),
                  _InfoCard(user: _user, cardColor: cardColor, subtitleColor: subtitleColor),
                  const SizedBox(height: 16),
                  _ActionsCard(cardColor: cardColor),
                ],
              ),
            ),
    );
  }
}

// ─── Avatar ───────────────────────────────────────────────────────────────────

class _Avatar extends StatelessWidget {
  final String name;
  const _Avatar({required this.name});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Container(
          width: 88,
          height: 88,
          decoration: const BoxDecoration(color: _primary, shape: BoxShape.circle),
          child: Center(
            child: Text(
              name.isNotEmpty ? name[0].toUpperCase() : 'U',
              style: const TextStyle(
                  color: Colors.white, fontSize: 36, fontWeight: FontWeight.bold),
            ),
          ),
        ),
        const SizedBox(height: 12),
        Text(name,
            style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
      ],
    );
  }
}

// ─── Info Card ────────────────────────────────────────────────────────────────

class _InfoCard extends StatelessWidget {
  final User? user;
  final Color cardColor;
  final Color subtitleColor;
  const _InfoCard(
      {required this.user,
      required this.cardColor,
      required this.subtitleColor});

  @override
  Widget build(BuildContext context) {
    final fullName =
        '${user?.name ?? ''} ${user?.surname ?? ''}'.trim();
    return _Card(
      color: cardColor,
      child: Column(
        children: [
          _InfoRow(
              icon: Icons.person_rounded,
              label: 'Username',
              value: user?.userName ?? '-',
              subtitleColor: subtitleColor),
          _Divider(),
          _InfoRow(
              icon: Icons.email_rounded,
              label: 'Email',
              value: user?.emailAddress ?? '-',
              subtitleColor: subtitleColor),
          _Divider(),
          _InfoRow(
              icon: Icons.badge_rounded,
              label: 'Full Name',
              value: fullName.isEmpty ? '-' : fullName,
              subtitleColor: subtitleColor),
        ],
      ),
    );
  }
}

// ─── Actions Card ─────────────────────────────────────────────────────────────

class _ActionsCard extends StatelessWidget {
  final Color cardColor;
  const _ActionsCard({required this.cardColor});

  @override
  Widget build(BuildContext context) {
    return _Card(
      color: cardColor,
      child: Column(
        children: [
          _ActionRow(
            icon: Icons.lock_outline_rounded,
            label: 'Change Password',
            onTap: () => Navigator.push(
              context,
              MaterialPageRoute(
                  builder: (_) => const ChangePasswordScreen()),
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Shared widgets ───────────────────────────────────────────────────────────

class _Card extends StatelessWidget {
  final Color color;
  final Widget child;
  const _Card({required this.color, required this.child});

  @override
  Widget build(BuildContext context) => Container(
        width: double.infinity,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
        decoration: BoxDecoration(
          color: color,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 8,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: child,
      );
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final Color subtitleColor;
  const _InfoRow(
      {required this.icon,
      required this.label,
      required this.value,
      required this.subtitleColor});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 14),
      child: Row(
        children: [
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
                color: _primaryLight, borderRadius: BorderRadius.circular(9)),
            child: Icon(icon, color: _primary, size: 18),
          ),
          const SizedBox(width: 12),
          Text(label, style: TextStyle(color: subtitleColor, fontSize: 13)),
          const Spacer(),
          Text(value,
              style: const TextStyle(
                  fontWeight: FontWeight.w500, fontSize: 13)),
        ],
      ),
    );
  }
}

class _ActionRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;
  const _ActionRow(
      {required this.icon, required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 14),
        child: Row(
          children: [
            Container(
              width: 36,
              height: 36,
              decoration: BoxDecoration(
                  color: _primaryLight,
                  borderRadius: BorderRadius.circular(9)),
              child: Icon(icon, color: _primary, size: 18),
            ),
            const SizedBox(width: 12),
            Text(label,
                style: const TextStyle(
                    fontWeight: FontWeight.w500, fontSize: 14)),
            const Spacer(),
            const Icon(Icons.chevron_right_rounded,
                color: Colors.black26, size: 20),
          ],
        ),
      ),
    );
  }
}

class _Divider extends StatelessWidget {
  @override
  Widget build(BuildContext context) =>
      const Divider(height: 1, thickness: 0.5);
}
