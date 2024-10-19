import 'package:curved_navigation_bar/curved_navigation_bar.dart';
import 'package:flutter/material.dart';

import '../analysis/production.dart';
import 'side_menu.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final screens = [
    const ProductionPage(),
    const ProductionPage(),
    const ProductionPage(),
  ];
  int _page = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const SideMenu(),
      appBar: AppBar(
        backgroundColor: Colors.deepOrange,
        title: const Text(
          'Dashboard',
          style: TextStyle(color: Colors.white),
        ),
      ),
      body: screens[_page],
      bottomNavigationBar: CurvedNavigationBar(
        backgroundColor: Colors.white,
        color: Colors.deepOrange,
        height: 60,
        items: const <Widget>[
          Icon(Icons.drive_eta, size: 20),
          Icon(Icons.receipt_long, size: 20),
          Icon(Icons.task, size: 20),
        ],
        animationDuration: const Duration(milliseconds: 200),
        animationCurve: Curves.ease,
        index: 1,
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
