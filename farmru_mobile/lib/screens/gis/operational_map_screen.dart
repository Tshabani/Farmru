import 'package:flutter/material.dart';
import '../../services/geo_service.dart';

class OperationalMapScreen extends StatefulWidget {
  const OperationalMapScreen({super.key});

  @override
  State<OperationalMapScreen> createState() => _OperationalMapScreenState();
}

class _OperationalMapScreenState extends State<OperationalMapScreen> {
  Map<String, dynamic>? mapData;
  bool loading = true;

  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    setState(() => loading = true);
    mapData = await GeoService.getOperationalMap();
    setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    final devices = (mapData?['devices'] as List<dynamic>?) ?? [];
    final facilities = (mapData?['facilities'] as List<dynamic>?) ?? [];

    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFFB7873B),
        title: const Text('Field operations', style: TextStyle(color: Colors.white)),
      ),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: load,
              child: ListView(
                padding: const EdgeInsets.all(12),
                children: [
                  Text('Facilities (${facilities.length})',
                      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                  ...facilities.map((f) => _tile(
                        f['name']?.toString() ?? 'Facility',
                        '${f['deviceCount']} devices · ${f['activeAlertCount']} alerts',
                        Icons.location_city,
                        Colors.green,
                      )),
                  const SizedBox(height: 16),
                  Text('Devices (${devices.length})',
                      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                  ...devices.map((d) => _tile(
                        d['displayText']?.toString() ?? 'Device',
                        '${d['isOnline'] == true ? 'Online' : 'Offline'} · ${d['latitude']}, ${d['longitude']}',
                        Icons.sensors,
                        d['isOnline'] == true ? Colors.blue : Colors.red,
                      )),
                ],
              ),
            ),
    );
  }

  Widget _tile(String title, String subtitle, IconData icon, Color color) {
    return Card(
      child: ListTile(
        leading: Icon(icon, color: color),
        title: Text(title),
        subtitle: Text(subtitle),
      ),
    );
  }
}
