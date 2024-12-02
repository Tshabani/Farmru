// ignore: file_names
import 'package:flutter/material.dart';

import '../../models/NodeAvgDataResponse.dart';

class SensorIndicators extends StatelessWidget {
  final NodeAvgData sensorData;

  const SensorIndicators({super.key, required this.sensorData});

  @override
  Widget build(BuildContext context) {
    // Extracting sensor data with null checks
    final temperature =
        sensorData.avgSoilTemperature?.toStringAsFixed(1) ?? "N/A";
    final solarPanelVoltage =
        sensorData.avgSolarPanelVoltage?.toStringAsFixed(1) ?? "N/A";
    final batteryVoltage =
        sensorData.avgBatteryVoltage?.toStringAsFixed(1) ?? "N/A";

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          SensorIndicator(
            icon: Icons.thermostat,
            label: "TEMPERATURE",
            value: "$temperature°C",
            description: "The soil's average temperature.",
          ),
          SensorIndicator(
            icon: Icons.light_mode,
            label: "SOLAR PANEL VOLTAGE",
            value: "$solarPanelVoltage V",
            description: "The voltage produced by the solar panel.",
          ),
          SensorIndicator(
            icon: Icons.battery_charging_full,
            label: "BATTERY VOLTAGE",
            value: "$batteryVoltage V",
            description: "The voltage level of the battery.",
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
  Widget build(BuildContext context) {
    return Card(
      elevation: 3,
      shape: RoundedRectangleBorder(
        borderRadius:
            BorderRadius.circular(20), // Rounded edges for modern look
      ),
      margin: const EdgeInsets.all(8), // Spacing between cards
      child: Padding(
        padding: const EdgeInsets.all(12.0), // Inner padding
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 45, color: Colors.blue),
            const SizedBox(height: 5), // Spacing between icon and value
            Text(
              value,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 5), // Spacing between value and label
            Text(
              label,
              style: const TextStyle(fontSize: 12, color: Colors.black87),
            ),
            const SizedBox(height: 5), // Spacing before description
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 5.0),
              child: Text(
                description,
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 10, color: Colors.grey[700]),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
