import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../models/nodeDataResponse.dart';

class CustomLineChart extends StatelessWidget {
  final List<NodeData> sensorData;

  const CustomLineChart({super.key, required this.sensorData});

  @override
  Widget build(BuildContext context) {
    return LineChart(
      sampleData(sensorData),
      duration: const Duration(milliseconds: 250),
    );
  }

  LineChartData sampleData(List<NodeData> data) {
    // Group data by minute
    Map<DateTime, double> groupedData = {};

    for (var element in data) {
      DateTime minute = DateTime(
          element.creationTime.year,
          element.creationTime.month,
          element.creationTime.day,
          element.creationTime.hour,
          element.creationTime.minute);
      if (groupedData.containsKey(minute)) {
        groupedData[minute] =
            (groupedData[minute]! + double.parse(element.soilTemperature)) / 2;
      } else {
        groupedData[minute] = double.parse(element.soilTemperature);
      }
    }

    // Convert map to FlSpot list
    List<FlSpot> spots = [];
    int index = 0;
    groupedData.forEach((key, value) {
      spots.add(FlSpot(index.toDouble(), value));
      index++;
    });

    return LineChartData(
      lineTouchData: const LineTouchData(enabled: !false),
      gridData: const FlGridData(show: false),
      titlesData: FlTitlesData(
        bottomTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: !true,
            reservedSize: 40,
            getTitlesWidget: (value, meta) => Text(
              DateFormat('HH:mm').format(DateTime.fromMillisecondsSinceEpoch(
                  (value.toInt() * 60 * 1000))),
              style: const TextStyle(fontSize: 12),
            ),
            interval: 10,
          ),
        ),
        rightTitles: const AxisTitles(
          sideTitles: SideTitles(showTitles: false),
        ),
        topTitles: const AxisTitles(
          sideTitles: SideTitles(showTitles: false),
        ),
        leftTitles: const AxisTitles(
          sideTitles: SideTitles(showTitles: true, reservedSize: 43),
        ),
      ),
      borderData: FlBorderData(
        show: true,
        border: const Border(
          bottom: BorderSide(color: Colors.grey, width: 2),
          left: BorderSide(color: Colors.transparent),
          right: BorderSide(color: Colors.transparent),
          top: BorderSide(color: Colors.transparent),
        ),
      ),
      lineBarsData: [
        LineChartBarData(
          isCurved: true,
          color: Colors.blue.withOpacity(0.5),
          barWidth: 2,
          isStrokeCapRound: true,
          dotData: const FlDotData(show: false),
          belowBarData: BarAreaData(
            show: true,
            color: Colors.blue.withOpacity(0.2),
          ),
          spots: spots,
        ),
      ],
      minX: 0,
      maxX: spots.length.toDouble(),
      maxY: data
              .map((e) => double.parse(e.soilTemperature))
              .reduce((a, b) => a > b ? a : b) +
          2,
      minY: 0,
    );
  }
}
