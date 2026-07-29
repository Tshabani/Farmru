import 'package:flutter/material.dart';

import '../../models/crop_models.dart';
import '../../models/nutrient_models.dart';
import '../../services/field_service.dart';
import '../../services/nutrient_service.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class NutrientBalanceScreen extends StatefulWidget {
  const NutrientBalanceScreen({super.key});

  @override
  State<NutrientBalanceScreen> createState() => _NutrientBalanceScreenState();
}

class _NutrientBalanceScreenState extends State<NutrientBalanceScreen> {
  bool _isLoading = true;
  List<FieldResult> _fields = [];
  FieldResult? _selectedField;
  NutrientBalanceSnapshotResult? _latest;
  List<NutrientBalanceSnapshotResult> _history = [];

  @override
  void initState() {
    super.initState();
    _loadFields();
  }

  Future<void> _loadFields() async {
    setState(() => _isLoading = true);
    final fields = await FieldService.GetAll();
    if (!mounted) return;
    setState(() {
      _fields = fields;
      _selectedField = fields.isNotEmpty ? fields.first : null;
    });

    if (_selectedField != null) {
      await _loadBalance(_selectedField!);
    } else {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _loadBalance(FieldResult field) async {
    setState(() => _isLoading = true);
    final latest = await NutrientService.GetLatest(field.id);
    final history = await NutrientService.GetHistory(field.id);
    if (!mounted) return;
    setState(() {
      _latest = latest;
      _history = history;
      _isLoading = false;
    });
  }

  Color _statusColor(int status) {
    switch (status) {
      case 0:
        return Colors.red.shade600; // Deficient
      case 2:
        return Colors.orange.shade700; // Surplus
      default:
        return Colors.green.shade600; // Adequate
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Nutrient Balance')),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : _fields.isEmpty
              ? const _EmptyState(message: 'No Fields have been set up yet.')
              : RefreshIndicator(
                  onRefresh: () => _loadBalance(_selectedField!),
                  color: _primary,
                  child: ListView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
                    children: [
                      if (_fields.length > 1)
                        DropdownButtonFormField<String>(
                          initialValue: _selectedField!.id,
                          decoration: const InputDecoration(labelText: 'Field', border: OutlineInputBorder()),
                          items: _fields
                              .map((f) => DropdownMenuItem(value: f.id, child: Text(f.name)))
                              .toList(),
                          onChanged: (id) {
                            final match = _fields.firstWhere((f) => f.id == id, orElse: () => _selectedField!);
                            setState(() => _selectedField = match);
                            _loadBalance(match);
                          },
                        ),
                      const SizedBox(height: 16),
                      if (_latest == null)
                        const _EmptyState(message: 'No nutrient balance data is available for this Field yet.')
                      else ...[
                        _NutrientStatusCard(
                          label: 'Nitrogen (N)',
                          sensed: _latest!.sensedNitrogen,
                          applied: _latest!.appliedNitrogenTrailing30d,
                          status: _latest!.nitrogenStatus,
                          color: _statusColor(_latest!.nitrogenStatus),
                        ),
                        const SizedBox(height: 12),
                        _NutrientStatusCard(
                          label: 'Phosphorus (P)',
                          sensed: _latest!.sensedPhosphorus,
                          applied: _latest!.appliedPhosphorusTrailing30d,
                          status: _latest!.phosphorusStatus,
                          color: _statusColor(_latest!.phosphorusStatus),
                        ),
                        const SizedBox(height: 12),
                        _NutrientStatusCard(
                          label: 'Potassium (K)',
                          sensed: _latest!.sensedPotassium,
                          applied: _latest!.appliedPotassiumTrailing30d,
                          status: _latest!.potassiumStatus,
                          color: _statusColor(_latest!.potassiumStatus),
                        ),
                      ],
                      if (_history.isNotEmpty) ...[
                        const SizedBox(height: 20),
                        Text('History', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w700, color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54), letterSpacing: 0.8)),
                        const SizedBox(height: 12),
                        ..._history.take(14).map((s) => Padding(
                              padding: const EdgeInsets.symmetric(vertical: 4),
                              child: Text(
                                '${s.snapshotDate.year}-${s.snapshotDate.month}-${s.snapshotDate.day}: N ${nutrientStatusLabel(s.nitrogenStatus)}, P ${nutrientStatusLabel(s.phosphorusStatus)}, K ${nutrientStatusLabel(s.potassiumStatus)}',
                                style: const TextStyle(fontSize: 12),
                              ),
                            )),
                      ],
                    ],
                  ),
                ),
    );
  }
}

class _NutrientStatusCard extends StatelessWidget {
  final String label;
  final double sensed;
  final double applied;
  final int status;
  final Color color;

  const _NutrientStatusCard({
    required this.label,
    required this.sensed,
    required this.applied,
    required this.status,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surface,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [BoxShadow(color: Colors.black.withValues(alpha: 0.05), blurRadius: 10, offset: const Offset(0, 4))],
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
                const SizedBox(height: 4),
                Text('Sensed: ${sensed.toStringAsFixed(1)}', style: const TextStyle(fontSize: 12)),
                Text('Applied (30d): ${applied.toStringAsFixed(1)} kg/ha', style: const TextStyle(fontSize: 12)),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
            decoration: BoxDecoration(color: color.withValues(alpha: 0.12), borderRadius: BorderRadius.circular(20)),
            child: Text(nutrientStatusLabel(status), style: TextStyle(color: color, fontSize: 12, fontWeight: FontWeight.w700)),
          ),
        ],
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  final String message;
  const _EmptyState({required this.message});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(color: _primaryLight, borderRadius: BorderRadius.circular(16)),
      child: Column(
        children: [
          const Icon(Icons.science_outlined, color: _primary, size: 32),
          const SizedBox(height: 8),
          Text(message, textAlign: TextAlign.center, style: const TextStyle(color: _primary)),
        ],
      ),
    );
  }
}
