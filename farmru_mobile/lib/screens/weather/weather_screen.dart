import 'package:flutter/material.dart';

import '../../models/crop_models.dart';
import '../../models/weather_models.dart';
import '../../services/node_service.dart';
import '../../services/weather_service.dart';

const _primary = Color(0xFFB7873B);
const _primaryLight = Color(0xFFF5E6C8);

class WeatherScreen extends StatefulWidget {
  const WeatherScreen({super.key});

  @override
  State<WeatherScreen> createState() => _WeatherScreenState();
}

class _WeatherScreenState extends State<WeatherScreen> {
  bool _isLoading = true;
  List<DisplayRef> _facilities = [];
  DisplayRef? _selectedFacility;
  WeatherObservationResult? _current;
  List<WeatherForecastResult> _forecast = [];

  @override
  void initState() {
    super.initState();
    _loadFacilities();
  }

  Future<void> _loadFacilities() async {
    setState(() => _isLoading = true);
    final nodes = await NodeService.GetAll();
    final seen = <String>{};
    final facilities = <DisplayRef>[];
    for (final node in nodes ?? []) {
      if (node.facility.id.isNotEmpty && seen.add(node.facility.id)) {
        facilities.add(DisplayRef(id: node.facility.id, displayText: node.facility.displayText));
      }
    }

    setState(() {
      _facilities = facilities;
      _selectedFacility = facilities.isNotEmpty ? facilities.first : null;
    });

    if (_selectedFacility != null) {
      await _loadWeather(_selectedFacility!);
    } else {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _loadWeather(DisplayRef facility) async {
    setState(() => _isLoading = true);
    final current = await WeatherService.GetCurrent(facility.id);
    final forecast = await WeatherService.GetForecast(facility.id);
    if (!mounted) return;
    setState(() {
      _current = current;
      _forecast = forecast;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Weather')),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _primary))
          : _facilities.isEmpty
              ? const _EmptyState(message: 'No Facility data available yet.')
              : RefreshIndicator(
                  onRefresh: () => _loadWeather(_selectedFacility!),
                  color: _primary,
                  child: SingleChildScrollView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (_facilities.length > 1) _FacilityPicker(
                          facilities: _facilities,
                          selected: _selectedFacility!,
                          onChanged: (f) {
                            setState(() => _selectedFacility = f);
                            _loadWeather(f);
                          },
                        ),
                        const SizedBox(height: 12),
                        _current == null
                            ? const _EmptyState(message: 'No weather data is available for this Facility yet.')
                            : _CurrentConditionsCard(observation: _current!),
                        const SizedBox(height: 20),
                        const _SectionLabel('7-Day Forecast'),
                        const SizedBox(height: 12),
                        _forecast.isEmpty
                            ? const _EmptyState(message: 'No forecast data is available for this Facility yet.')
                            : _ForecastStrip(days: _forecast),
                      ],
                    ),
                  ),
                ),
    );
  }
}

class _FacilityPicker extends StatelessWidget {
  final List<DisplayRef> facilities;
  final DisplayRef selected;
  final ValueChanged<DisplayRef> onChanged;

  const _FacilityPicker({required this.facilities, required this.selected, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return DropdownButtonFormField<String>(
      initialValue: selected.id,
      decoration: const InputDecoration(labelText: 'Facility', border: OutlineInputBorder()),
      items: facilities
          .map((f) => DropdownMenuItem(value: f.id, child: Text(f.displayText)))
          .toList(),
      onChanged: (id) {
        final match = facilities.firstWhere((f) => f.id == id, orElse: () => selected);
        onChanged(match);
      },
    );
  }
}

class _CurrentConditionsCard extends StatelessWidget {
  final WeatherObservationResult observation;
  const _CurrentConditionsCard({required this.observation});

  @override
  Widget build(BuildContext context) {
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
          BoxShadow(color: _primary.withValues(alpha: 0.35), blurRadius: 16, offset: const Offset(0, 6)),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('${observation.temperatureCelsius.toStringAsFixed(1)}°C',
                    style: const TextStyle(color: Colors.white, fontSize: 34, fontWeight: FontWeight.bold)),
                const SizedBox(height: 4),
                Text('Humidity ${observation.humidityPercent.toStringAsFixed(0)}%',
                    style: const TextStyle(color: Colors.white70, fontSize: 13)),
                if (observation.windSpeedKph != null)
                  Text('Wind ${observation.windSpeedKph!.toStringAsFixed(0)} kph',
                      style: const TextStyle(color: Colors.white70, fontSize: 13)),
              ],
            ),
          ),
          const Icon(Icons.wb_sunny_rounded, color: Colors.white, size: 48),
        ],
      ),
    );
  }
}

class _ForecastStrip extends StatelessWidget {
  final List<WeatherForecastResult> days;
  const _ForecastStrip({required this.days});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 110,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: days.length,
        separatorBuilder: (_, __) => const SizedBox(width: 10),
        itemBuilder: (context, index) {
          final day = days[index];
          final surface = Theme.of(context).colorScheme.surface;
          return Container(
            width: 88,
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: surface,
              borderRadius: BorderRadius.circular(14),
              boxShadow: [BoxShadow(color: Colors.black.withValues(alpha: 0.05), blurRadius: 8, offset: const Offset(0, 3))],
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text('${day.forecastFor.month}/${day.forecastFor.day}', style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w600)),
                const SizedBox(height: 6),
                Text('${day.tempMaxCelsius.toStringAsFixed(0)}°', style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold)),
                Text('${day.tempMinCelsius.toStringAsFixed(0)}°', style: TextStyle(fontSize: 12, color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.5))),
                const SizedBox(height: 4),
                Text('${day.precipitationProbabilityPercent}%', style: const TextStyle(fontSize: 11, color: Colors.blue)),
              ],
            ),
          );
        },
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
          const Icon(Icons.cloud_off_rounded, color: _primary, size: 32),
          const SizedBox(height: 8),
          Text(message, textAlign: TextAlign.center, style: const TextStyle(color: _primary)),
        ],
      ),
    );
  }
}
