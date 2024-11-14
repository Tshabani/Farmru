import 'package:flutter/material.dart';

import '../components/CloudDatabaseCard.dart';
import '../components/RealTimeDataInfoCard.dart';
import '../components/SensorIndicators.dart';
import '../components/SoilMoistureCard.dart';

class AverageReadingsPage extends StatefulWidget {
  const AverageReadingsPage({super.key});

  @override
  State<AverageReadingsPage> createState() => _AverageReadingsPageState();
}

class _AverageReadingsPageState extends State<AverageReadingsPage> {
  // late List<LogSheetItem>? _logSheets;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[200],
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            SoilMoistureCard(),
            CloudDatabaseCard(),
            const SizedBox(height: 10),
            SensorIndicators(),
            const SizedBox(height: 10),
            RealTimeDataCard()
          ],
        ),
      ),
    );
  }
}
