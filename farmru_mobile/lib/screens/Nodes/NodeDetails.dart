import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

class NodeDetailsPage extends StatefulWidget {
  const NodeDetailsPage({super.key});

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

  @override
  void initState() {
    super.initState();
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
          : cardView(),
    );
  }

  Widget cardView() {
    final charts = [
      ChartData(
        "Temperature",
        [
          const FlSpot(1, 25),
          const FlSpot(2, 27),
          const FlSpot(3, 30),
          const FlSpot(4, 20),
          const FlSpot(5, 35),
          const FlSpot(6, 33),
          const FlSpot(7, 36),
          const FlSpot(8, 38),
          const FlSpot(9, 40),
          const FlSpot(10, 42),
          const FlSpot(11, 41),
          const FlSpot(12, 100),
          const FlSpot(13, 45),
          const FlSpot(14, 47),
          const FlSpot(15, 50),
          const FlSpot(16, 52),
          const FlSpot(17, 51),
          const FlSpot(18, 3),
          const FlSpot(19, 55),
          const FlSpot(20, 58),
          const FlSpot(21, 55),
          const FlSpot(22, 57),
          const FlSpot(23, 59),
          const FlSpot(24, 61),
          const FlSpot(25, 64),
          const FlSpot(26, 2),
          const FlSpot(27, 65),
          const FlSpot(28, 68),
          const FlSpot(29, 70),
          const FlSpot(30, 72),
        ],
        Colors.blue,
      ),
      ChartData(
        "Soil PH",
        [
          const FlSpot(1, 6.5),
          const FlSpot(2, 6.8),
          const FlSpot(3, 6.9),
          const FlSpot(4, 6.7),
          const FlSpot(5, 6.6),
          const FlSpot(6, 6.8),
          const FlSpot(7, 7.0),
          const FlSpot(8, 7.2),
          const FlSpot(9, 6.9),
          const FlSpot(10, 7.1),
          const FlSpot(11, 7.0),
          const FlSpot(12, 6.8),
          const FlSpot(13, 6.7),
          const FlSpot(14, 6.5),
          const FlSpot(15, 6.6),
          const FlSpot(16, 6.8),
          const FlSpot(17, 6.9),
          const FlSpot(18, 7.1),
          const FlSpot(19, 7.2),
          const FlSpot(20, 7.0),
          const FlSpot(21, 6.9),
          const FlSpot(22, 6.7),
          const FlSpot(23, 6.8),
          const FlSpot(24, 6.6),
          const FlSpot(25, 6.5),
          const FlSpot(26, 6.7),
          const FlSpot(27, 6.9),
          const FlSpot(28, 6.8),
          const FlSpot(29, 6.6),
          const FlSpot(30, 6.7),
        ],
        Colors.green,
      ),
      ChartData(
        "Humidity",
        [
          const FlSpot(1, 70),
          const FlSpot(2, 72),
          const FlSpot(3, 75),
          const FlSpot(4, 73),
          const FlSpot(5, 78),
          const FlSpot(6, 80),
          const FlSpot(7, 79),
          const FlSpot(8, 81),
          const FlSpot(9, 85),
          const FlSpot(10, 88),
          const FlSpot(11, 86),
          const FlSpot(12, 82),
          const FlSpot(13, 79),
          const FlSpot(14, 77),
          const FlSpot(15, 75),
          const FlSpot(16, 76),
          const FlSpot(17, 78),
          const FlSpot(18, 80),
          const FlSpot(19, 82),
          const FlSpot(20, 81),
          const FlSpot(21, 79),
          const FlSpot(22, 77),
          const FlSpot(23, 78),
          const FlSpot(24, 80),
          const FlSpot(25, 83),
          const FlSpot(26, 85),
          const FlSpot(27, 88),
          const FlSpot(28, 86),
          const FlSpot(29, 84),
          const FlSpot(30, 82),
        ],
        Colors.teal,
      ),
      ChartData(
        "Phosphorus",
        [
          const FlSpot(1, 10),
          const FlSpot(2, 12),
          const FlSpot(3, 11),
          const FlSpot(4, 13),
          const FlSpot(5, 14),
          const FlSpot(6, 15),
          const FlSpot(7, 16),
          const FlSpot(8, 15),
          const FlSpot(9, 17),
          const FlSpot(10, 18),
          const FlSpot(11, 16),
          const FlSpot(12, 14),
          const FlSpot(13, 13),
          const FlSpot(14, 12),
          const FlSpot(15, 10),
          const FlSpot(16, 11),
          const FlSpot(17, 12),
          const FlSpot(18, 13),
          const FlSpot(19, 15),
          const FlSpot(20, 16),
          const FlSpot(21, 14),
          const FlSpot(22, 13),
          const FlSpot(23, 12),
          const FlSpot(24, 14),
          const FlSpot(25, 15),
          const FlSpot(26, 16),
          const FlSpot(27, 17),
          const FlSpot(28, 16),
          const FlSpot(29, 15),
          const FlSpot(30, 14),
        ],
        Colors.purple,
      ),
      ChartData(
        "Potassium",
        [
          const FlSpot(1, 20),
          const FlSpot(2, 22),
          const FlSpot(3, 24),
          const FlSpot(4, 23),
          const FlSpot(5, 25),
          const FlSpot(6, 27),
          const FlSpot(7, 29),
          const FlSpot(8, 28),
          const FlSpot(9, 30),
          const FlSpot(10, 32),
          const FlSpot(11, 31),
          const FlSpot(12, 29),
          const FlSpot(13, 28),
          const FlSpot(14, 26),
          const FlSpot(15, 25),
          const FlSpot(16, 27),
          const FlSpot(17, 28),
          const FlSpot(18, 30),
          const FlSpot(19, 32),
          const FlSpot(20, 31),
          const FlSpot(21, 29),
          const FlSpot(22, 27),
          const FlSpot(23, 28),
          const FlSpot(24, 30),
          const FlSpot(25, 32),
          const FlSpot(26, 31),
          const FlSpot(27, 33),
          const FlSpot(28, 32),
          const FlSpot(29, 30),
          const FlSpot(30, 29),
        ],
        Colors.orange,
      ),
      ChartData(
        "Nitrogen",
        [
          const FlSpot(1, 5),
          const FlSpot(2, 6),
          const FlSpot(3, 7),
          const FlSpot(4, 6),
          const FlSpot(5, 8),
          const FlSpot(6, 9),
          const FlSpot(7, 8),
          const FlSpot(8, 10),
          const FlSpot(9, 11),
          const FlSpot(10, 12),
          const FlSpot(11, 10),
          const FlSpot(12, 9),
          const FlSpot(13, 8),
          const FlSpot(14, 7),
          const FlSpot(15, 6),
          const FlSpot(16, 7),
          const FlSpot(17, 8),
          const FlSpot(18, 9),
          const FlSpot(19, 10),
          const FlSpot(20, 11),
          const FlSpot(21, 10),
          const FlSpot(22, 9),
          const FlSpot(23, 8),
          const FlSpot(24, 7),
          const FlSpot(25, 8),
          const FlSpot(26, 9),
          const FlSpot(27, 10),
          const FlSpot(28, 11),
          const FlSpot(29, 10),
          const FlSpot(30, 9),
        ],
        Colors.red,
      ),
    ];

    return SingleChildScrollView(
      child: Card(
        elevation: 3,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        margin: const EdgeInsets.symmetric(horizontal: 5, vertical: 5),
        child: Padding(
          padding: const EdgeInsets.all(5),
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
                horizontalInterval: 10,
                verticalInterval: 10,
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
