// ignore_for_file: file_names, non_constant_identifier_names

import 'package:flutter/material.dart';

class CustomContainer extends StatelessWidget {
  final String text;
  final String SubText;
  final Widget page;

  const CustomContainer({
    super.key,
    required this.text,
    required this.SubText,
    required this.page,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () {
        Navigator.push(
          context,
          MaterialPageRoute(builder: (context) => page),
        );
      },
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(20),
          color: const Color(0xFFB7873B),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              text,
              style: const TextStyle(color: Colors.white, fontSize: 13),
            ),
            Text(
              SubText,
              style: const TextStyle(color: Colors.white, fontSize: 10),
            )
          ],
        ),
      ),
    );
  }
}
