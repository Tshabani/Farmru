import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../models/nodeDataResponse.dart';
import '../../models/nodeResponse.dart';
import '../../services/node_service.dart';
import '../../services/alert_service.dart';
import '../../models/alert_models.dart';
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
  bool _isLoading = true;
  late List<NodeData>? nodeData = [];
  Map<String, dynamic>? deviceDetail;
  List<AlertItem>? deviceAlerts;

  @override
  void initState() {
    super.initState();
    loadData();
  }

  Future<void> loadData() async {
    setState(() => _isLoading = true);
    final results = await Future.wait([
      NodeService.GetNodeDataByNode(widget.node),
      NodeService.GetDeviceDetail(widget.node.id),
      AlertService.getAlertsByNode(widget.node.id),
    ]);
    nodeData = results[0] as List<NodeData>?;
    deviceDetail = results[1] as Map<String, dynamic>?;
    deviceAlerts = results[2] as List<AlertItem>?;
    setState(() => _isLoading = false);
  }

  Color _healthColor(int? status) {
    switch (status ?? widget.node.healthStatus) {
      case 1:
        return Colors.green;
      case 2:
        return Colors.orange;
      case 3:
        return Colors.red;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    final isOnline = deviceDetail?['isOnline'] ?? widget.node.isOnline;
    final battery = deviceDetail?['batteryLevel'] ?? widget.node.batteryLevel;
    final lastSeen = deviceDetail?['lastSeenAt'] ?? widget.node.lastSeenAt;
    final healthStatus = deviceDetail?['healthStatus'] ?? widget.node.healthStatus;

    return Scaffold(
      appBar: AppBar(
        title: Text(widget.node.displayName ?? widget.node.serialNumber),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Icon(
                                Icons.circle,
                                size: 12,
                                color: isOnline == true ? Colors.green : Colors.grey,
                              ),
                              const SizedBox(width: 8),
                              Text(
                                isOnline == true ? 'Online' : 'Offline',
                                style: const TextStyle(fontWeight: FontWeight.bold),
                              ),
                              const Spacer(),
                              Container(
                                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                decoration: BoxDecoration(
                                  color: _healthColor(healthStatus).withValues(alpha: 0.15),
                                  borderRadius: BorderRadius.circular(4),
                                ),
                                child: Text(
                                  widget.node.healthLabel,
                                  style: TextStyle(color: _healthColor(healthStatus)),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 12),
                          _statusRow('Serial', widget.node.serialNumber),
                          if (widget.node.facility.displayText.isNotEmpty)
                            _statusRow('Facility', widget.node.facility.displayText),
                          if (battery != null)
                            _statusRow('Battery', '${battery.toString()}%'),
                          if (lastSeen != null)
                            _statusRow('Last seen', lastSeen.toString()),
                          if (widget.node.firmwareVersion != null)
                            _statusRow('Firmware', widget.node.firmwareVersion!),
                        ],
                      ),
                    ),
                  ),
                  if (deviceAlerts != null && deviceAlerts!.isNotEmpty) ...[
                    const SizedBox(height: 12),
                    const Text('Alerts', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                    Card(
                      child: ListView.builder(
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        itemCount: deviceAlerts!.length,
                        itemBuilder: (context, i) {
                          final a = deviceAlerts![i];
                          return ListTile(
                            dense: true,
                            title: Text(a.title),
                            subtitle: Text(a.severityLabel),
                          );
                        },
                      ),
                    ),
                  ],
                  const SizedBox(height: 12),
                  const Text('Telemetry', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  cardView(nodeData ?? []),
                ],
              ),
            ),
    );
  }

  Widget _statusRow(String label, String value) {
    final onSurface = Theme.of(context).colorScheme.onSurface;
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        children: [
          SizedBox(
            width: 90,
            child: Text(label,
                style: TextStyle(
                    color: onSurface.withValues(alpha: 0.54))),
          ),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  Widget cardView(List<NodeData> sensorData) {
    if (sensorData.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Center(child: Text('No sensor data available.')),
        ),
      );
    }

    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: CustomLineChart(sensorData: sensorData),
      ),
    );
  }
}
