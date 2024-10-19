// ignore_for_file: file_names

import 'package:flutter/material.dart';

class CustomButton extends StatelessWidget {
  final String text;
  final Function onPressed;
  final EdgeInsets padding;
  final BorderRadius borderRadius;
  final Color color;
  final Color textColor;

  const CustomButton({
    super.key,
    required this.text,
    required this.onPressed,
    this.padding = const EdgeInsets.symmetric(horizontal: 50),
    this.borderRadius = const BorderRadius.all(Radius.circular(10)),
    this.color = Colors.deepOrange,
    this.textColor = Colors.white,
  });

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: () => onPressed(),
      style: ElevatedButton.styleFrom(
        padding: padding,
        backgroundColor: color,
        shape: RoundedRectangleBorder(
          borderRadius: borderRadius,
        ),
        minimumSize: const Size(double.infinity, 35),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: textColor,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}
