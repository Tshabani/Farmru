// ignore_for_file: library_private_types_in_public_api, use_super_parameters

import 'package:flutter/material.dart';

class DropdownComponent extends StatefulWidget {
  final List<String> items;
  final ValueChanged<String?> onItemSelected;

  const DropdownComponent({
    Key? key,
    required this.items,
    required this.onItemSelected,
  }) : super(key: key);

  @override
  _DropdownComponentState createState() => _DropdownComponentState();
}

class _DropdownComponentState extends State<DropdownComponent> {
  String? _selectedItem;

  void _onItemSelected(String? selectedItem) {
    setState(() {
      _selectedItem = selectedItem;
    });

    widget.onItemSelected(selectedItem);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        SizedBox(
          width: double.infinity,
          child: DropdownButton<String>(
            value: _selectedItem,
            isExpanded: true,
            hint: const Text("Select a tenant "),
            items: widget.items.map((String item) {
              return DropdownMenuItem<String>(
                value: item,
                child: Text(item),
              );
            }).toList(),
            onChanged: (String? newValue) {
              _onItemSelected(newValue);
            },
          ),
        ),
      ],
    );
  }
}
