import 'package:curved_navigation_bar/curved_navigation_bar.dart';
import 'package:farmru_mobile/screens/home/home_grid_screen.dart';
import 'package:flutter/material.dart';

import '../Nodes/AverageReadings.dart';
import 'side_menu.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final screens = [
    const AverageReadingsPage(),
    const HomeGridScreen(),
  ];
  int _page = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const SideMenu(),
      appBar: AppBar(title: const Text('Dashboard')),
      body: screens[_page],
      bottomNavigationBar: CurvedNavigationBar(
        backgroundColor: Theme.of(context).scaffoldBackgroundColor,
        color: const Color(0xFFB7873B),
        height: 60,
        items: const <Widget>[
          Icon(Icons.data_thresholding_outlined, size: 20, color: Colors.white),
          Icon(Icons.data_exploration_outlined, size: 20, color: Colors.white),
        ],
        animationDuration: const Duration(milliseconds: 200),
        animationCurve: Curves.ease,
        index: _page,
        onTap: (index) => setState(() => _page = index),
      ),
    );
  }
}
