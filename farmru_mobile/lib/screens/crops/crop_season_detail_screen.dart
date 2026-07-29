import 'package:flutter/material.dart';

import '../../models/crop_models.dart';
import '../../services/crop_season_service.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class CropSeasonDetailScreen extends StatefulWidget {
  final String cropSeasonId;
  const CropSeasonDetailScreen({super.key, required this.cropSeasonId});

  @override
  State<CropSeasonDetailScreen> createState() => _CropSeasonDetailScreenState();
}

class _CropSeasonDetailScreenState extends State<CropSeasonDetailScreen> {
  bool _isLoading = true;
  CropSeasonDetailResult? _season;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final season = await CropSeasonService.GetDetail(widget.cropSeasonId);
    if (!mounted) return;
    setState(() {
      _season = season;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(_season?.cropType.displayText ?? 'Crop Season')),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : _season == null
              ? const Center(child: Text('Crop season not found.'))
              : RefreshIndicator(
                  onRefresh: _load,
                  color: _primary,
                  child: SingleChildScrollView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _SummaryCard(season: _season!),
                        const SizedBox(height: 20),
                        if (_season!.harvest != null) ...[
                          const _SectionLabel('Harvest'),
                          const SizedBox(height: 12),
                          _HarvestCard(harvest: _season!.harvest!),
                          const SizedBox(height: 20),
                        ],
                        const _SectionLabel('Growth Stage Timeline'),
                        const SizedBox(height: 12),
                        ..._season!.stageEvents.map((e) => _StageRow(event: e)),
                      ],
                    ),
                  ),
                ),
    );
  }
}

class _SummaryCard extends StatelessWidget {
  final CropSeasonDetailResult season;
  const _SummaryCard({required this.season});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(color: _primaryLight, borderRadius: BorderRadius.circular(16)),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Field: ${season.field.displayText}', style: const TextStyle(fontWeight: FontWeight.w600)),
          const SizedBox(height: 4),
          Text('Status: ${season.statusLabel}', style: const TextStyle(color: _primary, fontWeight: FontWeight.w600)),
          const SizedBox(height: 4),
          Text('Planted: ${season.plantingDate.year}-${season.plantingDate.month}-${season.plantingDate.day}'),
          Text('Expected harvest: ${season.expectedHarvestDate.year}-${season.expectedHarvestDate.month}-${season.expectedHarvestDate.day}'),
        ],
      ),
    );
  }
}

class _HarvestCard extends StatelessWidget {
  final HarvestRecordResult harvest;
  const _HarvestCard({required this.harvest});

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
          const Icon(Icons.agriculture_rounded, color: _primary, size: 28),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('${harvest.actualYieldKg.toStringAsFixed(1)} kg', style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                Text('Harvested ${harvest.harvestDate.year}-${harvest.harvestDate.month}-${harvest.harvestDate.day}',
                    style: TextStyle(fontSize: 12, color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54))),
                if (harvest.qualityGrade != null) Text('Grade: ${harvest.qualityGrade}', style: const TextStyle(fontSize: 12)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _StageRow extends StatelessWidget {
  final GrowthStageEventResult event;
  const _StageRow({required this.event});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        children: [
          Container(width: 10, height: 10, decoration: const BoxDecoration(color: _primary, shape: BoxShape.circle)),
          const SizedBox(width: 14),
          Expanded(child: Text(event.stageLabel, style: const TextStyle(fontWeight: FontWeight.w600))),
          Text('${event.observedDate.year}-${event.observedDate.month}-${event.observedDate.day}',
              style: TextStyle(fontSize: 12, color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54))),
        ],
      ),
    );
  }
}

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Text(
        text,
        style: TextStyle(
          fontSize: 13,
          fontWeight: FontWeight.w700,
          color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.54),
          letterSpacing: 0.8,
        ),
      );
}
