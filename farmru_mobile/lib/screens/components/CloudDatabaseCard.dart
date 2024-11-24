// ignore_for_file: file_names

import 'package:flutter/material.dart';

class CloudDatabaseCard extends StatelessWidget {
  const CloudDatabaseCard({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 0, 30, 0),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
              topRight: Radius.circular(90),
              bottomRight: Radius.circular(90)), // Rounded corners
        ),
        margin: const EdgeInsets.symmetric(vertical: 10),
        child: Row(
          children: [
            const Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "Cloud Database",
                    style: TextStyle(
                        color: Colors.white,
                        fontSize: 12,
                        fontWeight: FontWeight.bold),
                  ),
                  Text(
                    "Soil Moisture: 60%\nHumidity: 50%\nAmbient Temperature: 25°C\nLight Intensity: 20 Lux",
                    style: TextStyle(color: Colors.white, fontSize: 10),
                  ),
                ],
              ),
            ),
            Container(
              decoration: const BoxDecoration(
                color: Colors.white,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.wifi, // This icon resembles the earth symbol
                color: Colors.green[400],
                size: 70,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
