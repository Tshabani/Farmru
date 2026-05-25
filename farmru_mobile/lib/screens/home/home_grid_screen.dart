import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/material.dart';
import '../../models/category_model.dart';
import '../../models/nodeResponse.dart';
import '../../services/node_service.dart';
import '../Nodes/NodeDetails.dart';
import '../alerts/alerts_screen.dart';
import '../monitoring/monitoring_screen.dart';
import '../components/CustomContainer.dart';
import '../components/carousel_card.dart';

class HomeGridScreen extends StatefulWidget {
  const HomeGridScreen({super.key});

  @override
  State<HomeGridScreen> createState() => _HomeGridScreenState();
}

class _HomeGridScreenState extends State<HomeGridScreen> {
  late List<NodeResult>? _node = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    getNodes();
  }

  Future<void> getNodes() async {
    setState(() {
      _isLoading = !false;
    });
    _node = (await NodeService.GetAll());
    setState(() {
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          CarouselSlider(
            options: CarouselOptions(
              aspectRatio: 2.0,
              viewportFraction: 0.9,
              enlargeCenterPage: true,
              enlargeStrategy: CenterPageEnlargeStrategy.height,
              height: 250,
            ),
            items: Category.categories
                .map((category) => CarouselCard(category: category))
                .toList(),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 10),
            child: ListTile(
              leading: const Icon(Icons.monitor_heart, color: Color(0xFFB7873B)),
              title: const Text('Operational monitoring'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () => Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const MonitoringScreen()),
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 10),
            child: ListTile(
              leading: const Icon(Icons.notifications_active, color: Color(0xFFB7873B)),
              title: const Text('Operational alerts'),
              trailing: const Icon(Icons.chevron_right),
              onTap: () => Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const AlertsScreen()),
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(10.0),
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : GridView(
                    scrollDirection: Axis.vertical,
                    shrinkWrap: true,
                    gridDelegate:
                        const SliverGridDelegateWithFixedCrossAxisCount(
                            crossAxisCount: 4,
                            mainAxisSpacing: 20,
                            crossAxisSpacing: 20),
                    children: _node?.map((node) {
                          return CustomContainer(
                            text: node.displayName?.isNotEmpty == true
                                ? node.displayName!
                                : node.facility.displayText,
                            SubText: '${node.isOnline ? "● " : "○ "}${node.serialNumber}',
                            page: NodeDetailsPage(
                              node: node,
                            ), // Pass the node to the details page
                          );
                        }).toList() ??
                        [], // Ensure to handle null or empty list case
                  ),
          ),
        ],
      ),
    );
  }
}
