// ignore: duplicate_ignore
// ignore: file_names
// ignore_for_file: file_names

import 'package:flutter/material.dart';

class CustomLabel extends StatelessWidget {
  final String leadingText;
  final String lastText;

  const CustomLabel(
      {super.key, required this.leadingText, required this.lastText});

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 25,
      padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 0),
      child: Row(
        children: [
          Text(leadingText,
              style: const TextStyle(fontWeight: FontWeight.bold)),
          Expanded(
            child: Text(lastText,
                style: const TextStyle(fontWeight: FontWeight.bold),
                textAlign: TextAlign.end),
          ),
        ],
      ),
    );
  }
}
