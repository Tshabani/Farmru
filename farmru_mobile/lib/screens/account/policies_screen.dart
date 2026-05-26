import 'package:flutter/material.dart';

const _primary = Color(0xFFB7873B);

class PoliciesScreen extends StatelessWidget {
  const PoliciesScreen({super.key});

  static const _policies = [
    _Policy(
      icon: Icons.privacy_tip_rounded,
      title: 'Privacy Policy',
      summary:
          'We collect only the data necessary to operate the platform. Your farm data is never sold or shared with third parties.',
    ),
    _Policy(
      icon: Icons.gavel_rounded,
      title: 'Terms of Service',
      summary:
          'By using Farmru you agree to use the platform responsibly and in accordance with applicable laws and regulations.',
    ),
    _Policy(
      icon: Icons.cookie_rounded,
      title: 'Data Retention',
      summary:
          'Sensor readings are retained for 24 months. Account data is retained for the lifetime of your subscription.',
    ),
    _Policy(
      icon: Icons.security_rounded,
      title: 'Security',
      summary:
          'All data is transmitted over TLS 1.2+. Passwords are hashed using industry-standard algorithms and never stored in plaintext.',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Policies'),
      ),
      body: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: _policies.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (_, i) => _PolicyCard(policy: _policies[i]),
      ),
    );
  }
}

class _Policy {
  final IconData icon;
  final String title;
  final String summary;
  const _Policy({required this.icon, required this.title, required this.summary});
}

class _PolicyCard extends StatelessWidget {
  final _Policy policy;
  const _PolicyCard({required this.policy});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
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
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 42,
            height: 42,
            decoration: BoxDecoration(
              color: const Color(0xFFF5E6C8),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(policy.icon, color: _primary, size: 20),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(policy.title,
                    style: const TextStyle(
                        fontWeight: FontWeight.w700, fontSize: 14)),
                const SizedBox(height: 6),
                Text(policy.summary,
                    style: TextStyle(
                        fontSize: 13,
                        color: Theme.of(context)
                            .colorScheme
                            .onSurface
                            .withValues(alpha: 0.54),
                        height: 1.5)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
