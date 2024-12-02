import 'package:flutter/material.dart';
import 'package:percent_indicator/circular_percent_indicator.dart';

import '../../models/NodeAvgDataResponse.dart';

class SoilMoistureCard extends StatelessWidget {
  final NodeAvgData sensorData;

  const SoilMoistureCard({super.key, required this.sensorData});

  @override
  Widget build(BuildContext context) {
    // Safely parse avgMoisture from sensorData
    final moisture = sensorData.avgMoisture ?? 0.0;
    final moisturePercentage = (moisture / 100).clamp(0.0, 1.0);

    return Padding(
      padding: const EdgeInsets.fromLTRB(30, 30, 0, 0),
      child: Container(
        padding: const EdgeInsets.all(10),
        height: 100.0,
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
            topLeft: Radius.circular(90),
            bottomLeft: Radius.circular(90),
          ),
        ),
        margin: const EdgeInsets.symmetric(vertical: 10),
        child: Row(
          children: [
            // Circular Icon with Earth Symbol
            CircularPercentIndicator(
              radius: 37.0,
              lineWidth: 5.0,
              percent: moisturePercentage,
              center: Text(
                "${(moisturePercentage * 100).toStringAsFixed(0)}%",
                style: const TextStyle(
                  fontSize: 20.0,
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                ),
              ),
              progressColor: Colors.green[400],
              backgroundColor: Colors.grey[300]!,
            ),
            const SizedBox(width: 15), // Space between icon and text
            Expanded(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    "Soil Moisture (0-100%)",
                    style: TextStyle(color: Colors.white, fontSize: 15),
                  ),
                  Text(
                    moisture > 60
                        ? "WET"
                        : moisture > 30
                            ? "MOIST"
                            : "DRY",
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
