// ignore_for_file: file_names

import 'package:flutter/material.dart';

class RealTimeDataCard extends StatelessWidget {
  const RealTimeDataCard({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 300,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.green[400],
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          // Circular Icon with Earth Symbol
          Container(
            padding: const EdgeInsets.all(12),
            decoration: const BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
            ),
            child: Icon(
              Icons.public, // This icon resembles the earth symbol
              color: Colors.green[400],
              size: 32,
            ),
          ),
          const SizedBox(width: 16), // Space between icon and text
          // Text Section
          const Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  "Real time data",
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                SizedBox(height: 8),
                Text(
                  "The farm state function displays real time data of PH levels from NPK tester, relative humidity, soil moisture, environmental light intensity and humidity.",
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 14,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
