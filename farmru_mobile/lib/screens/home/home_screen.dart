import 'package:curved_navigation_bar/curved_navigation_bar.dart';
import 'package:flutter/material.dart';

import '../Incidents/IncidentsList.dart';
import '../Nodes/AverageReadings.dart';
import '../Nodes/NodeDetails.dart';
import '../Nodes/NodeList.dart';
import '../Task/TaskList.dart';
import 'side_menu.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final screens = [
    const AverageReadingsPage(),
    const NodeDetailsPage(),
    const IncidentListPage(),
    const TasksListPage(),
  ];
  int _page = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const SideMenu(),
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text(
          'Dashboard',
          style: TextStyle(color: Colors.black),
        ),
      ),
      body: screens[_page],
      bottomNavigationBar: CurvedNavigationBar(
        backgroundColor: Colors.white,
        color: const Color(0xFFB7873B),
        height: 60,
        items: const <Widget>[
          Icon(Icons.data_thresholding_outlined, size: 20),
          Icon(Icons.data_thresholding_outlined, size: 20),
          Icon(Icons.receipt_long_outlined, size: 20),
          Icon(Icons.task_alt_outlined, size: 20),
        ],
        animationDuration: const Duration(milliseconds: 200),
        animationCurve: Curves.ease,
        index: 0,
        onTap: (index) {
          //Handle button tap
          setState(() {
            _page = index;
          });
        },
      ),
    );
  }
}
