import 'package:flutter/material.dart';

class IncidentDetailsPage extends StatefulWidget {
  const IncidentDetailsPage({super.key});

  @override
  State<IncidentDetailsPage> createState() => _IncidentDetailsPageState();
}

class _IncidentDetailsPageState extends State<IncidentDetailsPage> {
  // late List<LogSheetItem>? _logSheets;
  final bool _isLoading = true;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(child: CircularProgressIndicator()),
    );
  }
}
