import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../models/nodeDataResponse.dart';
import '../../models/nodeResponse.dart';
import '../../services/node_service.dart';
import '../components/lineChart.dart';

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

    return Center(
      child: Card(
        elevation: 5,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: CustomLineChart(
            sensorData: sensorData,
          ),
        ),
      ),
    );
  }
}
