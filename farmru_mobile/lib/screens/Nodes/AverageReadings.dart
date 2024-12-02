import 'package:flutter/material.dart';

import '../../models/NodeAvgDataResponse.dart';
import '../../services/node_service.dart';
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
  NodeAvgData? _sensorData;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    getAvgNodes();
  }

  Future<void> getAvgNodes() async {
    setState(() {
      _isLoading = !false;
    });
    _sensorData = await NodeService.GetSensorData();
    setState(() {
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[200],
      body: _isLoading
          ? const Center(
              child: CircularProgressIndicator(),
            )
          : SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const SizedBox(height: 10),
                  SoilMoistureCard(
                    sensorData: _sensorData!,
                  ),
                  const SizedBox(height: 20),
                  CloudDatabaseCard(sensorData: _sensorData!),
                  const SizedBox(height: 20),
                  SensorIndicators(sensorData: _sensorData!),
                  const SizedBox(height: 10),
                  const RealTimeDataCard()
                ],
              ),
            ),
    );
  }
}
