import 'package:flutter/material.dart';
import '../../services/geo_service.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class OperationalMapScreen extends StatefulWidget {
  const OperationalMapScreen({super.key});

  @override
  State<OperationalMapScreen> createState() => _OperationalMapScreenState();
}

class _OperationalMapScreenState extends State<OperationalMapScreen> {
  Map<String, dynamic>? _mapData;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    _mapData = await GeoService.getOperationalMap();
    if (mounted) setState(() => _loading = false);
  }

  @override
  Widget build(BuildContext context) {
    final devices = (_mapData?['devices'] as List<dynamic>?) ?? [];
    final facilities = (_mapData?['facilities'] as List<dynamic>?) ?? [];

    return Scaffold(
      appBar: AppBar(title: const Text('Field Operations')),
      body: _loading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: _load,
              color: _primary,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  _SummaryRow(
                    facilityCount: facilities.length,
                    deviceCount: devices.length,
                    onlineCount: devices
                        .where((d) => d['isOnline'] == true)
                        .length,
                  ),
                  const SizedBox(height: 20),
                  if (facilities.isNotEmpty) ...[
                    _SectionLabel(
                        'Facilities', facilities.length, Icons.location_city_rounded),
                    const SizedBox(height: 10),
                    ...facilities.map((f) => _FacilityCard(data: f)),
                    const SizedBox(height: 20),
                  ],
                  if (devices.isNotEmpty) ...[
                    _SectionLabel(
                        'Devices', devices.length, Icons.sensors_rounded),
                    const SizedBox(height: 10),
                    ...devices.map((d) => _DeviceCard(data: d)),
                  ],
                  if (facilities.isEmpty && devices.isEmpty)
                    const _EmptyState(),
                ],
              ),
            ),
    );
  }
}

// ─── Summary Row ──────────────────────────────────────────────────────────────

class _SummaryRow extends StatelessWidget {
  final int facilityCount;
  final int deviceCount;
  final int onlineCount;
  const _SummaryRow(
      {required this.facilityCount,
      required this.deviceCount,
      required this.onlineCount});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
            child: _StatCard(
                label: 'Facilities', value: facilityCount, color: _primary)),
        const SizedBox(width: 10),
        Expanded(
            child: _StatCard(
                label: 'Devices', value: deviceCount, color: Colors.blueGrey)),
        const SizedBox(width: 10),
        Expanded(
            child: _StatCard(
                label: 'Online', value: onlineCount, color: Colors.green)),
      ],
    );
  }
}

class _StatCard extends StatelessWidget {
  final String label;
  final int value;
  final Color color;
  const _StatCard(
      {required this.label, required this.value, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 14),
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
      child: Column(
        children: [
          Text('$value',
              style: TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                  color: color)),
          const SizedBox(height: 2),
          Text(label,
              style: TextStyle(fontSize: 11,
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.45))),
        ],
      ),
    );
  }
}

// ─── Section Label ────────────────────────────────────────────────────────────

class _SectionLabel extends StatelessWidget {
  final String title;
  final int count;
  final IconData icon;
  const _SectionLabel(this.title, this.count, this.icon);

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, size: 16, color: _primary),
        const SizedBox(width: 8),
        Text(
          '$title ($count)',
          style: TextStyle(
              fontSize: 13,
              fontWeight: FontWeight.w700,
              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54),
              letterSpacing: 0.5),
        ),
      ],
    );
  }
}

// ─── Facility Card ────────────────────────────────────────────────────────────

class _FacilityCard extends StatelessWidget {
  final Map<String, dynamic> data;
  const _FacilityCard({required this.data});

  @override
  Widget build(BuildContext context) {
    final name = data['name']?.toString() ?? 'Facility';
    final deviceCount = data['deviceCount'] ?? 0;
    final alertCount = data['activeAlertCount'] ?? 0;

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(14),
        border: const Border(left: BorderSide(color: _primary, width: 4)),
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
            child: const Icon(Icons.location_city_rounded,
                color: _primary, size: 20),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(name,
                    style: const TextStyle(
                        fontWeight: FontWeight.w600, fontSize: 14)),
                const SizedBox(height: 4),
                Row(
                  children: [
                    _Chip(
                        label: '$deviceCount devices',
                        color: Colors.blueGrey),
                    const SizedBox(width: 6),
                    if (alertCount > 0)
                      _Chip(
                          label: '$alertCount alerts',
                          color: Colors.red),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Device Card ──────────────────────────────────────────────────────────────

class _DeviceCard extends StatelessWidget {
  final Map<String, dynamic> data;
  const _DeviceCard({required this.data});

  @override
  Widget build(BuildContext context) {
    final name = data['displayText']?.toString() ?? 'Device';
    final isOnline = data['isOnline'] == true;
    final lat = data['latitude']?.toString() ?? '-';
    final lng = data['longitude']?.toString() ?? '-';
    final color = isOnline ? Colors.green : Colors.red;

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border(left: BorderSide(color: color, width: 4)),
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
              color: color.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(Icons.sensors_rounded, color: color, size: 20),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(name,
                    style: const TextStyle(
                        fontWeight: FontWeight.w600, fontSize: 14)),
                const SizedBox(height: 4),
                Row(
                  children: [
                    _Chip(
                        label: isOnline ? 'Online' : 'Offline',
                        color: color),
                    const SizedBox(width: 6),
                    Text('$lat, $lng',
                        style: TextStyle(
                            fontSize: 11,
                            color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.38))),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Chip ─────────────────────────────────────────────────────────────────────

class _Chip extends StatelessWidget {
  final String label;
  final Color color;
  const _Chip({required this.label, required this.color});

  @override
  Widget build(BuildContext context) => Container(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(20),
        ),
        child: Text(label,
            style: TextStyle(
                fontSize: 10, color: color, fontWeight: FontWeight.w600)),
      );
}

// ─── Empty State ──────────────────────────────────────────────────────────────

class _EmptyState extends StatelessWidget {
  const _EmptyState();

  @override
  Widget build(BuildContext context) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.map_outlined,
                size: 56, color: onSurface.withValues(alpha: 0.15)),
            const SizedBox(height: 16),
            Text('No field data',
                style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: onSurface.withValues(alpha: 0.38))),
            const SizedBox(height: 6),
            Text('No facilities or devices found.',
                style: TextStyle(
                    fontSize: 12, color: onSurface.withValues(alpha: 0.26))),
          ],
        ),
      ),
    );
  }
}
