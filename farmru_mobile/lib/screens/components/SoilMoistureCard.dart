import 'package:flutter/material.dart';

class SoilMoistureCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.all(16),
      child: Column(
        children: [
          CircularProgressIndicator(
            value: 0.6, // 60%
            backgroundColor: Colors.grey[300],
            valueColor: AlwaysStoppedAnimation<Color>(Colors.blue),
            strokeWidth: 8,
          ),
          SizedBox(height: 8),
          Text(
            "Soil Moisture (0-100%)",
            style: TextStyle(fontSize: 16),
          ),
          Text(
            "WET",
            style: TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.bold,
              color: Colors.blue,
            ),
          ),
        ],
      ),
    );
  }
}
