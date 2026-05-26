import 'package:flutter/material.dart';
import 'package:percent_indicator/circular_percent_indicator.dart';

import '../../models/NodeAvgDataResponse.dart';
import '../../services/node_service.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class AverageReadingsPage extends StatefulWidget {
  const AverageReadingsPage({super.key});

  @override
  State<AverageReadingsPage> createState() => _AverageReadingsPageState();
}

class _AverageReadingsPageState extends State<AverageReadingsPage> {
  NodeAvgData? _sensorData;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    getAvgNodes();
  }

  Future<void> getAvgNodes() async {
    setState(() => _isLoading = true);
    _sensorData = await NodeService.GetSensorData();
    if (mounted) setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : RefreshIndicator(
              onRefresh: getAvgNodes,
              color: _primary,
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.fromLTRB(16, 20, 16, 24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const _SectionLabel('Overview'),
                    const SizedBox(height: 12),
                    _MoistureHeroCard(data: _sensorData!),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: _MetricCard(
                            icon: Icons.thermostat_rounded,
                            label: 'Soil Temp',
                            value:
                                '${_sensorData!.avgSoilTemperature.toStringAsFixed(1)}°C',
                            iconColor: Colors.orange,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _MetricCard(
                            icon: Icons.science_rounded,
                            label: 'Soil pH',
                            value:
                                _sensorData!.avgSoilPh.toStringAsFixed(1),
                            iconColor: Colors.purple,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _MetricCard(
                            icon: Icons.light_mode_rounded,
                            label: 'Solar Voltage',
                            value:
                                '${_sensorData!.avgSolarPanelVoltage.toStringAsFixed(1)} V',
                            iconColor: Colors.amber.shade700,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _MetricCard(
                            icon: Icons.battery_charging_full_rounded,
                            label: 'Battery',
                            value:
                                '${_sensorData!.avgBatteryVoltage.toStringAsFixed(1)} V',
                            iconColor: Colors.green.shade700,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 24),
                    const _SectionLabel('Soil Nutrients (NPK)'),
                    const SizedBox(height: 12),
                    _NutrientPanel(data: _sensorData!),
                    const SizedBox(height: 24),
                    const _SectionLabel('System Status'),
                    const SizedBox(height: 12),
                    const _StatusBanner(),
                  ],
                ),
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

// ─── Moisture Hero Card ───────────────────────────────────────────────────────

class _MoistureHeroCard extends StatelessWidget {
  final NodeAvgData data;
  const _MoistureHeroCard({required this.data});

  String get _label {
    final m = data.avgMoisture;
    if (m > 60) return 'WET';
    if (m > 30) return 'MOIST';
    return 'DRY';
  }

  Color get _labelColor {
    final m = data.avgMoisture;
    if (m > 60) return Colors.blue.shade300;
    if (m > 30) return Colors.green.shade300;
    return Colors.orange.shade300;
  }

  @override
  Widget build(BuildContext context) {
    final pct = (data.avgMoisture / 100).clamp(0.0, 1.0);
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [Color(0xFFB7873B), Color(0xFFD4A55A)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: _primary.withValues(alpha: 0.35),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        children: [
          CircularPercentIndicator(
            radius: 52,
            lineWidth: 7,
            percent: pct,
            center: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  '${(pct * 100).toStringAsFixed(0)}%',
                  style: const TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
                Text(
                  _label,
                  style: TextStyle(
                    fontSize: 10,
                    fontWeight: FontWeight.w600,
                    color: _labelColor,
                  ),
                ),
              ],
            ),
            progressColor: Colors.white,
            backgroundColor: Colors.white24,
            circularStrokeCap: CircularStrokeCap.round,
          ),
          const SizedBox(width: 20),
          const Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Soil Moisture',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                SizedBox(height: 6),
                Text(
                  'Average across all active nodes',
                  style: TextStyle(color: Colors.white70, fontSize: 12),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Metric Card ──────────────────────────────────────────────────────────────

class _MetricCard extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final Color iconColor;

  const _MetricCard({
    required this.icon,
    required this.label,
    required this.value,
    required this.iconColor,
  });

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: iconColor.withValues(alpha: 0.12),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: iconColor, size: 22),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  value,
                  style: const TextStyle(
                    fontSize: 17,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  label,
                  style: TextStyle(
                      fontSize: 11,
                      color: onSurface.withValues(alpha: 0.45)),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

// ─── Nutrient Panel ───────────────────────────────────────────────────────────

class _NutrientPanel extends StatelessWidget {
  final NodeAvgData data;
  const _NutrientPanel({required this.data});

  @override
  Widget build(BuildContext context) {
    final nutrients = [
      _NutrientEntry(
          'Nitrogen (N)', data.avgNitrogen, Colors.green.shade600, 50),
      _NutrientEntry(
          'Phosphorus (P)', data.avgPhosphorus, Colors.blue.shade600, 50),
      _NutrientEntry(
          'Potassium (K)', data.avgPotassium, Colors.purple.shade600, 50),
      _NutrientEntry('Soil pH', data.avgSoilPh, _primary, 14),
    ];
    final surface = Theme.of(context).colorScheme.surface;
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        children: nutrients
            .map((n) => Padding(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  child: _NutrientRow(entry: n),
                ))
            .toList(),
      ),
    );
  }
}

class _NutrientEntry {
  final String label;
  final double value;
  final Color color;
  final double max;
  const _NutrientEntry(this.label, this.value, this.color, this.max);
}

class _NutrientRow extends StatelessWidget {
  final _NutrientEntry entry;
  const _NutrientRow({required this.entry});

  @override
  Widget build(BuildContext context) {
    final pct = (entry.value / entry.max).clamp(0.0, 1.0);
    final unit = entry.label.contains('pH') ? '' : ' mg/kg';
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(entry.label,
                style: TextStyle(
                    fontSize: 13, color: onSurface.withValues(alpha: 0.87))),
            Text(
              '${entry.value.toStringAsFixed(1)}$unit',
              style: TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.bold,
                  color: entry.color),
            ),
          ],
        ),
        const SizedBox(height: 6),
        ClipRRect(
          borderRadius: BorderRadius.circular(6),
          child: LinearProgressIndicator(
            value: pct,
            backgroundColor: onSurface.withValues(alpha: 0.08),
            valueColor: AlwaysStoppedAnimation<Color>(entry.color),
            minHeight: 8,
          ),
        ),
      ],
    );
  }
}

// ─── Status Banner ────────────────────────────────────────────────────────────

class _StatusBanner extends StatelessWidget {
  const _StatusBanner();

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final bg = isDark ? _primary.withValues(alpha: 0.15) : _primaryLight;
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: bg,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: _primary.withValues(alpha: 0.3)),
      ),
      child: Row(
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: const BoxDecoration(
              color: _primary,
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.wifi_rounded,
                color: Colors.white, size: 24),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Real-time Sync Active',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                    color: onSurface.withValues(alpha: 0.87),
                  ),
                ),
                const SizedBox(height: 3),
                Text(
                  'Displaying live averages across all field nodes — pull down to refresh.',
                  style: TextStyle(
                      fontSize: 11,
                      color: onSurface.withValues(alpha: 0.54)),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
