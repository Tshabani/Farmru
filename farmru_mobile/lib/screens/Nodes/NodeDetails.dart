import 'package:flutter/material.dart';

class NodeDetailsPage extends StatefulWidget {
  const NodeDetailsPage({super.key});

  @override
  State<NodeDetailsPage> createState() => _NodeDetailsPageState();
}

class _NodeDetailsPageState extends State<NodeDetailsPage> {
  // late List<LogSheetItem>? _logSheets;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: const Center(child: CircularProgressIndicator()),
    );
  }
}
