// ignore_for_file: file_names

import 'package:flutter/material.dart';

class CustomContainer extends StatelessWidget {
  final IconData icon;
  final String text;
  final Widget page;

  const CustomContainer({
    Key? key,
    required this.icon,
    required this.text,
    required this.page,
  }) : super(key: key);

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
          color: Colors.deepOrange,
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              icon,
              size: 23,
              color: Colors.white,
            ),
            Text(
              text,
              style: const TextStyle(color: Colors.white, fontSize: 13),
            )
          ],
        ),
      ),
    );
  }
}
