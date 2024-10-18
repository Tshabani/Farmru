// ignore_for_file: file_names

import 'package:flutter/material.dart';
import 'package:flutter_typeahead/flutter_typeahead.dart';

import '../../models/EntityWithDisplayNameDto.dart';

class EntitySearchField extends StatelessWidget {
  final TextEditingController controller;
  final ValueChanged<EntityWithDisplayNameDto?> onSuggestionSelected;
  final Future<List<EntityWithDisplayNameDto>> Function(String)
      suggestionsCallback;
  final String labelText;
  final String hintText;

  const EntitySearchField({
    Key? key,
    required this.controller,
    required this.onSuggestionSelected,
    required this.suggestionsCallback,
    required this.labelText,
    required this.hintText,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return TypeAheadField<EntityWithDisplayNameDto?>(
      hideSuggestionsOnKeyboardHide: false,
      debounceDuration: const Duration(milliseconds: 20),
      textFieldConfiguration: TextFieldConfiguration(
        controller: controller,
        decoration: InputDecoration(
          border: const OutlineInputBorder(),
          labelText: labelText,
          hintText: hintText,
        ),
      ),
      suggestionsCallback: suggestionsCallback,
      itemBuilder: (context, EntityWithDisplayNameDto? suggestion) {
        final entity = suggestion!;
        return ListTile(
          title: Text(entity.displayName ?? ''),
        );
      },
      noItemsFoundBuilder: (context) => const SizedBox(
        height: 100,
        child: Center(
          child: Text(
            'No items found.',
            style: TextStyle(fontSize: 24),
          ),
        ),
      ),
      onSuggestionSelected: onSuggestionSelected,
    );
  }
}
