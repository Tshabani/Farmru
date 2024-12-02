import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../models/nodeDataResponse.dart';
import '../../models/nodeResponse.dart';
import '../../services/node_service.dart';

class NodeDetailsPage extends StatefulWidget {
  final NodeResult node;

  const NodeDetailsPage({
    super.key,
    required this.node,
  });

  @override
  State<NodeDetailsPage> createState() => _NodeDetailsPageState();
}

class ChartData {
  final String title;
  final List<FlSpot> data;
  final Color color;

  ChartData(this.title, this.data, this.color);
}

class _NodeDetailsPageState extends State<NodeDetailsPage> {
  bool _isLoading = !true;
  late List<NodeData>? nodeData = [];

  @override
  void initState() {
    super.initState();
    getNodeData();
  }

  getNodeData() async {
    setState(() {
      _isLoading = !false;
    });

    nodeData = await NodeService.GetNodeDataByNode(widget.node);

    setState(() {
      _isLoading = false; // Reset loading state
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text(
          'Sensor data',
          style: TextStyle(color: Colors.white),
        ),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () => Navigator.of(context).pop(),
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : cardView(nodeData ?? []),
    );
  }

  Widget cardView(List<NodeData> sensorData) {
    if (sensorData.isEmpty) {
      return const Center(
        child: Text('No sensor data available.'),
      );
    }

    // Mapping the sensor data to FlSpot values for charts
    final charts = [
      ChartData(
        "Temperature",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(entry.key.toDouble(),
                double.parse(entry.value.soilTemperature ?? '0')))
            .toList(),
        Colors.blue,
      ),
      ChartData(
        "Soil PH",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(
                entry.key.toDouble(), double.parse(entry.value.soilPh ?? '0')))
            .toList(),
        Colors.green,
      ),
      ChartData(
        "Humidity",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(entry.key.toDouble(),
                double.parse(entry.value.moisture ?? '0')))
            .toList(),
        Colors.teal,
      ),
      ChartData(
        "Phosphorus",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(entry.key.toDouble(),
                double.parse(entry.value.phosphorus ?? '0')))
            .toList(),
        Colors.purple,
      ),
      ChartData(
        "Potassium",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(entry.key.toDouble(),
                double.parse(entry.value.potassium ?? '0')))
            .toList(),
        Colors.orange,
      ),
      ChartData(
        "Nitrogen",
        sensorData
            .asMap()
            .entries
            .map((entry) => FlSpot(entry.key.toDouble(),
                double.parse(entry.value.nitrogen ?? '0')))
            .toList(),
        Colors.red,
      ),
    ];

    return SingleChildScrollView(
      child: Card(
        elevation: 3,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        margin: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
        child: Padding(
          padding: const EdgeInsets.all(0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: charts
                .map(
                    (chart) => buildChart(chart.title, chart.data, chart.color))
                .toList(),
          ),
        ),
      ),
    );
  }

  Widget buildChart(String title, List<FlSpot> data, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        Text(
          title,
          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
        ),
        const SizedBox(height: 10),
        SizedBox(
          height: 200,
          child: LineChart(
            LineChartData(
              lineBarsData: [
                LineChartBarData(
                  spots: data,
                  color: color,
                  dotData: const FlDotData(show: false),
                ),
              ],
              titlesData: const FlTitlesData(
                rightTitles:
                    AxisTitles(sideTitles: SideTitles(showTitles: false)),
                topTitles:
                    AxisTitles(sideTitles: SideTitles(showTitles: false)),
              ),
              gridData: const FlGridData(
                show: !true,
                horizontalInterval: 5,
                verticalInterval: 2,
              ),
              minY: 0,
              minX: 0,
            ),
          ),
        ),
      ],
    );
  }
}
