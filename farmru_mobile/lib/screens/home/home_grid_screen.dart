import 'package:flutter/material.dart';
import '../../models/category_model.dart';
import '../../models/nodeResponse.dart';
import '../../services/node_service.dart';
import '../Nodes/NodeDetails.dart';
import '../alerts/alerts_screen.dart';
import '../monitoring/monitoring_screen.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class HomeGridScreen extends StatefulWidget {
  const HomeGridScreen({super.key});

  @override
  State<HomeGridScreen> createState() => _HomeGridScreenState();
}

class _HomeGridScreenState extends State<HomeGridScreen> {
  List<NodeResult>? _nodes = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    getNodes();
  }

  Future<void> getNodes() async {
    setState(() => _isLoading = true);
    _nodes = await NodeService.GetAll();
    if (mounted) setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: RefreshIndicator(
        onRefresh: getNodes,
        color: _primary,
        child: SingleChildScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _HeroCarousel(),
              const SizedBox(height: 8),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const _SectionLabel('Operations'),
                    const SizedBox(height: 10),
                    _OperationTile(
                      icon: Icons.monitor_heart_rounded,
                      label: 'Operational Monitoring',
                      subtitle: 'Device health & telemetry status',
                      onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (_) => const MonitoringScreen())),
                    ),
                    const SizedBox(height: 8),
                    _OperationTile(
                      icon: Icons.notifications_active_rounded,
                      label: 'Operational Alerts',
                      subtitle: 'Active warnings & critical events',
                      onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (_) => const AlertsScreen())),
                    ),
                    const SizedBox(height: 20),
                    const _SectionLabel('Field Nodes'),
                    const SizedBox(height: 10),
                    _isLoading
                        ? const Center(
                            child: Padding(
                              padding: EdgeInsets.all(24),
                              child: CircularProgressIndicator(color: _primary),
                            ),
                          )
                        : (_nodes == null || _nodes!.isEmpty)
                            ? const _EmptyNodes()
                            : GridView.builder(
                                shrinkWrap: true,
                                physics: const NeverScrollableScrollPhysics(),
                                itemCount: _nodes!.length,
                                gridDelegate:
                                    const SliverGridDelegateWithFixedCrossAxisCount(
                                  crossAxisCount: 2,
                                  mainAxisSpacing: 12,
                                  crossAxisSpacing: 12,
                                  childAspectRatio: 1.4,
                                ),
                                itemBuilder: (context, i) =>
                                    _NodeCard(node: _nodes![i]),
                              ),
                    const SizedBox(height: 24),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// ─── Hero Carousel ────────────────────────────────────────────────────────────

class _HeroCarousel extends StatefulWidget {
  @override
  State<_HeroCarousel> createState() => _HeroCarouselState();
}

class _HeroCarouselState extends State<_HeroCarousel> {
  int _current = 0;

  @override
  Widget build(BuildContext context) {
    final categories = Category.categories;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Column(
      children: [
        SizedBox(
          height: 200,
          child: PageView.builder(
            itemCount: categories.length,
            onPageChanged: (i) => setState(() => _current = i),
            itemBuilder: (context, i) {
              final cat = categories[i];
              return Padding(
                padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(20),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      Image.network(cat.imageUrl,
                          fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => Container(
                              color: onSurface.withValues(alpha: 0.08))),
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            colors: [
                              Colors.black.withValues(alpha: 0.55),
                              Colors.transparent,
                            ],
                            begin: Alignment.bottomCenter,
                            end: Alignment.topCenter,
                          ),
                        ),
                      ),
                      Positioned(
                        bottom: 14,
                        left: 16,
                        child: Text(
                          cat.name,
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            shadows: [
                              Shadow(blurRadius: 4, color: Colors.black54)
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(
            categories.length,
            (i) => AnimatedContainer(
              duration: const Duration(milliseconds: 200),
              margin: const EdgeInsets.symmetric(horizontal: 3),
              width: _current == i ? 18 : 6,
              height: 6,
              decoration: BoxDecoration(
                color: _current == i
                    ? _primary
                    : Theme.of(context)
                        .colorScheme
                        .onSurface
                        .withValues(alpha: 0.2),
                borderRadius: BorderRadius.circular(3),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

// ─── Operation Tile ───────────────────────────────────────────────────────────

class _OperationTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final String subtitle;
  final VoidCallback onTap;

  const _OperationTile({
    required this.icon,
    required this.label,
    required this.subtitle,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: surface,
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
              width: 44,
              height: 44,
              decoration: BoxDecoration(
                color: _primaryLight,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, color: _primary, size: 22),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(label,
                      style: const TextStyle(
                          fontWeight: FontWeight.w600, fontSize: 14)),
                  const SizedBox(height: 2),
                  Text(subtitle,
                      style: TextStyle(
                          fontSize: 11,
                          color: onSurface.withValues(alpha: 0.45))),
                ],
              ),
            ),
            Icon(Icons.chevron_right_rounded,
                color: onSurface.withValues(alpha: 0.26)),
          ],
        ),
      ),
    );
  }
}

// ─── Node Card ────────────────────────────────────────────────────────────────

class _NodeCard extends StatelessWidget {
  final NodeResult node;
  const _NodeCard({required this.node});

  Color get _statusColor => node.isOnline ? Colors.green : Colors.grey;

  @override
  Widget build(BuildContext context) {
    final name = node.displayName?.isNotEmpty == true
        ? node.displayName!
        : node.facility.displayText;
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return InkWell(
      onTap: () => Navigator.push(
        context,
        MaterialPageRoute(builder: (_) => NodeDetailsPage(node: node)),
      ),
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: surface,
          borderRadius: BorderRadius.circular(14),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 8,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.sensors_rounded, color: _primary, size: 18),
                const Spacer(),
                Container(
                  width: 8,
                  height: 8,
                  decoration: BoxDecoration(
                    color: _statusColor,
                    shape: BoxShape.circle,
                  ),
                ),
              ],
            ),
            const Spacer(),
            Text(
              name,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                  color: onSurface.withValues(alpha: 0.87)),
            ),
            const SizedBox(height: 2),
            Text(
              node.serialNumber,
              style: TextStyle(
                  fontSize: 10, color: onSurface.withValues(alpha: 0.38)),
            ),
          ],
        ),
      ),
    );
  }
}

// ─── Empty state ──────────────────────────────────────────────────────────────

class _EmptyNodes extends StatelessWidget {
  const _EmptyNodes();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          children: [
            Icon(Icons.sensors_off_rounded,
                size: 48, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 12),
            Text('No nodes found',
                style:
                    TextStyle(color: onSurface.withValues(alpha: 0.38))),
          ],
        ),
      ),
    );
  }
}

// ─── Section Label ────────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Text(
        text,
        style: TextStyle(
          fontSize: 13,
          fontWeight: FontWeight.w700,
          color: Theme.of(context)
              .colorScheme
              .onSurface
              .withValues(alpha: 0.54),
          letterSpacing: 0.8,
        ),
      );
}
