// ignore_for_file: file_names

import 'package:flutter/material.dart';

class RealTimeDataCard extends StatelessWidget {
  const RealTimeDataCard({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(30, 0, 0, 0),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: const BoxDecoration(
          color: Color(0xFFB7873B),
          borderRadius: BorderRadius.only(
              topLeft: Radius.circular(90), bottomLeft: Radius.circular(90)),
        ),
        margin: const EdgeInsets.symmetric(vertical: 10),
        child: Row(
          children: [
            // Circular Icon with Earth Symbol
            Container(
              decoration: const BoxDecoration(
                color: Colors.white,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.public, // This icon resembles the earth symbol
                color: Colors.green[400],
                size: 70,
              ),
            ),
            const SizedBox(width: 7), // Space between icon and text
            // Text Section
            const Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "Real time data",
                    style: TextStyle(
                        color: Colors.black,
                        fontSize: 12,
                        fontWeight: FontWeight.bold),
                  ),
                  Text(
                    "The farm state function displays real time data of PH levels from NPK tester, relative humidity, soil moisture, environmental light intensity and humidity.",
                    style: TextStyle(color: Colors.black, fontSize: 10),
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
