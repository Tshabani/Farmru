import 'package:flutter/material.dart';
import 'package:percent_indicator/percent_indicator.dart';

class SoilMoistureCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(30, 30, 0, 0),
      child: Container(
        padding: const EdgeInsets.all(10),
        height: 100.0,
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
              topLeft: Radius.circular(90), bottomLeft: Radius.circular(90)),
        ),
        margin: const EdgeInsets.symmetric(vertical: 10),
        child: Row(
          children: [
            // Circular Icon with Earth Symbol
            Center(
              heightFactor: 0.8,
              widthFactor: 0.85,
              child: CircularPercentIndicator(
                radius: 37.0,
                lineWidth: 5.0,
                percent: 0.2,
                center: const Text(
                  "+60%",
                  style: TextStyle(
                    fontSize: 20.0,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
                progressColor: Colors.green[400],
                backgroundColor: Colors.grey[300]!,
              ),
            ),
            const SizedBox(width: 15), // Space between icon and text
            const Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "Soil Moisture (0-100%)",
                    style: TextStyle(color: Colors.white, fontSize: 15),
                  ),
                  Text(
                    "WET.",
                    style: TextStyle(color: Colors.white, fontSize: 18),
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
