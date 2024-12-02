import 'package:flutter/material.dart';
import '../../models/NodeAvgDataResponse.dart';

class CloudDatabaseCard extends StatelessWidget {
  final NodeAvgData sensorData;

  const CloudDatabaseCard({super.key, required this.sensorData});

  @override
  Widget build(BuildContext context) {
    // Safely extracting and formatting sensor data with units
    final soilPh = sensorData.avgSoilPh?.toStringAsFixed(1) ?? "N/A";
    final potassium = sensorData.avgPotassium?.toStringAsFixed(1) ?? "N/A";
    final phosphorus = sensorData.avgPhosphorus?.toStringAsFixed(1) ?? "N/A";
    final nitrogen = sensorData.avgNitrogen?.toStringAsFixed(1) ?? "N/A";

    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 0, 30, 0),
      child: Container(
        padding: const EdgeInsets.all(8),
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
            topRight: Radius.circular(90),
            bottomRight: Radius.circular(90),
          ),
        ),
        margin: const EdgeInsets.symmetric(vertical: 0),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    "Cloud Database",
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 13,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 0), // Spacing between title and data
                  Text(
                    "Soil pH: $soilPh\n"
                    "Potassium: $potassium ppm\n"
                    "Phosphorus: $phosphorus ppm\n"
                    "Nitrogen: $nitrogen ppm",
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 10,
                    ),
                  ),
                ],
              ),
            ),
            Container(
              width: 70, // Ensures consistent size for icon container
              height: 70,
              decoration: const BoxDecoration(
                color: Colors.white,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.wifi,
                color: Colors.green[400],
                size: 40, // Scaled icon size
              ),
            ),
          ],
        ),
      ),
    );
  }
}
