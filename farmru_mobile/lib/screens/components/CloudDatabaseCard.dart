// ignore_for_file: file_names

import 'package:flutter/material.dart';

class CloudDatabaseCard extends StatelessWidget {
  const CloudDatabaseCard({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 0, 30, 0),
      child: Container(
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
              topRight: Radius.circular(90),
              bottomRight: Radius.circular(90)), // Rounded corners
        ),
        padding: const EdgeInsets.all(10),
        margin: const EdgeInsets.symmetric(vertical: 10),
        child: const Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.cloud, color: Colors.black, size: 36),
                SizedBox(width: 8),
                Text(
                  "Cloud Database",
                  style: TextStyle(fontSize: 20, color: Colors.black),
                ),
              ],
            ),
            SizedBox(height: 5),
            Text(
              "Soil Moisture: 60%\nHumidity: 50%\nAmbient Temperature: 25°C\nLight Intensity: 20 Lux",
              style: TextStyle(color: Colors.black),
            ),
          ],
        ),
      ),
    );
  }
}
