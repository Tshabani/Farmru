// ignore: file_names
import 'package:flutter/material.dart';

class SensorIndicators extends StatelessWidget {
  const SensorIndicators({super.key});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          SensorIndicator(
            icon: Icons.water_drop,
            label: "HUMIDITY",
            value: "86%",
            description: "The amount of water vapor in the air.",
          ),
          SensorIndicator(
            icon: Icons.thermostat,
            label: "TEMPERATURE",
            value: "25°C",
            description: "Ambient air temperature.",
          ),
          SensorIndicator(
            icon: Icons.light_mode,
            label: "LIGHT INTENSITY",
            value: "20 Lux",
            description: "Environmental light level.",
          ),
        ],
      ),
    );
  }
}

class SensorIndicator extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final String description;

  const SensorIndicator({
    super.key,
    required this.icon,
    required this.label,
    required this.value,
    required this.description,
  });

  @override
  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 3,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20), // Optional for rounded edges
      ),
      margin: const EdgeInsets.all(5), // Optional for spacing
      child: Padding(
        padding: const EdgeInsets.all(8.0), // Optional for inner padding
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 40, color: Colors.blue),
            Text(value,
                style:
                    const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
            Text(label, style: const TextStyle(fontSize: 13)),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 2.0),
              child: Text(
                description,
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 8, color: Colors.grey[700]),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
