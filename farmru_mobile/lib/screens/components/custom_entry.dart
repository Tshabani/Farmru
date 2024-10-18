import 'package:flutter/material.dart';

class CustomEntry extends StatefulWidget {
  final String labelText;
  final String placeholder;
  final bool isPasswordTextField;
  final TextEditingController controller;
  final TextInputType? keybordType;

  const CustomEntry({
    super.key,
    required this.labelText,
    required this.placeholder,
    required this.controller,
    this.isPasswordTextField = false,
    this.keybordType,
  });

  @override
  State<CustomEntry> createState() => _CustomEntryState();
}

class _CustomEntryState extends State<CustomEntry> {
  bool isObscurePassword = true;
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: TextFormField(
        keyboardType: widget.keybordType ?? TextInputType.text,
        controller: widget.controller,
        obscureText: widget.isPasswordTextField ? isObscurePassword : false,
        decoration: InputDecoration(
          suffixIcon: widget.isPasswordTextField
              ? IconButton(
                  icon: const Icon(Icons.remove_red_eye, color: Colors.grey),
                  onPressed: () {
                    setState(
                      () {
                        isObscurePassword = !isObscurePassword;
                      },
                    );
                  },
                )
              : null,
          contentPadding: const EdgeInsets.only(bottom: 5),
          labelText: widget.labelText,
          floatingLabelBehavior: FloatingLabelBehavior.always,
          hintText: widget.placeholder,
          hintStyle: const TextStyle(
              fontSize: 16, fontWeight: FontWeight.bold, color: Colors.grey),
        ),
      ),
    );
  }
}
