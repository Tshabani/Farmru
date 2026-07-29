import 'package:flutter/material.dart';

import '../../models/crop_models.dart';
import '../../services/crop_season_service.dart';
import '../../services/field_service.dart';
import 'crop_season_detail_screen.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class CropSeasonListScreen extends StatefulWidget {
  const CropSeasonListScreen({super.key});

  @override
  State<CropSeasonListScreen> createState() => _CropSeasonListScreenState();
}

class _CropSeasonListScreenState extends State<CropSeasonListScreen> {
  bool _isLoading = true;
  List<FieldResult> _fields = [];
  FieldResult? _selectedField;
  List<CropSeasonResult> _seasons = [];

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
      await _loadSeasons(_selectedField!);
    } else {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _loadSeasons(FieldResult field) async {
    setState(() => _isLoading = true);
    final seasons = await CropSeasonService.GetByField(field.id);
    if (!mounted) return;
    setState(() {
      _seasons = seasons;
      _isLoading = false;
    });
  }

  Color _statusColor(int status) {
    switch (status) {
      case 1:
        return Colors.green.shade600; // Growing
      case 2:
        return Colors.blue.shade600; // Harvested
      case 3:
        return Colors.grey.shade600; // Closed
      default:
        return Colors.orange.shade600; // Planned
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Crop Seasons')),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : _fields.isEmpty
              ? const _EmptyState(message: 'No Fields have been set up yet.')
              : RefreshIndicator(
                  onRefresh: () => _loadSeasons(_selectedField!),
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
                            _loadSeasons(match);
                          },
                        ),
                      const SizedBox(height: 12),
                      if (_seasons.isEmpty)
                        const _EmptyState(message: 'No Crop Seasons have been planted on this Field yet.')
                      else
                        ..._seasons.map((season) => _SeasonCard(
                              season: season,
                              statusColor: _statusColor(season.status),
                              onTap: () => Navigator.push(
                                context,
                                MaterialPageRoute(builder: (_) => CropSeasonDetailScreen(cropSeasonId: season.id)),
                              ),
                            )),
                    ],
                  ),
                ),
    );
  }
}

class _SeasonCard extends StatelessWidget {
  final CropSeasonResult season;
  final Color statusColor;
  final VoidCallback onTap;

  const _SeasonCard({required this.season, required this.statusColor, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final surface = Theme.of(context).colorScheme.surface;
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: surface,
            borderRadius: BorderRadius.circular(16),
            boxShadow: [BoxShadow(color: Colors.black.withValues(alpha: 0.05), blurRadius: 10, offset: const Offset(0, 4))],
          ),
          child: Row(
            children: [
              Container(
                width: 10,
                height: 10,
                decoration: BoxDecoration(color: statusColor, shape: BoxShape.circle),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(season.cropType.displayText, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
                    const SizedBox(height: 2),
                    Text('Planted ${season.plantingDate.year}-${season.plantingDate.month}-${season.plantingDate.day}',
                        style: TextStyle(fontSize: 12, color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54))),
                  ],
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(color: statusColor.withValues(alpha: 0.12), borderRadius: BorderRadius.circular(20)),
                child: Text(season.statusLabel, style: TextStyle(color: statusColor, fontSize: 11, fontWeight: FontWeight.w600)),
              ),
              const Icon(Icons.chevron_right_rounded, size: 20),
            ],
          ),
        ),
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
          const Icon(Icons.grass_rounded, color: _primary, size: 32),
          const SizedBox(height: 8),
          Text(message, textAlign: TextAlign.center, style: const TextStyle(color: _primary)),
        ],
      ),
    );
  }
}
