// ignore_for_file: file_names

import 'package:flutter/material.dart';
import 'package:flutter_typeahead/flutter_typeahead.dart';

typedef SuggestionCallback<T> = Future<List<T>> Function(String);

typedef ItemBuilder<T> = Widget Function(BuildContext, T);

typedef OnSuggestionSelected<T> = void Function(T);

class AutoCompleteTextField<T> extends StatelessWidget {
  const AutoCompleteTextField({
    Key? key,
    required this.controller,
    required this.decoration,
    required this.suggestionsCallback,
    required this.itemBuilder,
    required this.onSuggestionSelected,
    this.hideSuggestionsOnKeyboardHide = false,
  }) : super(key: key);

  final TextEditingController controller;
  final InputDecoration decoration;
  final SuggestionCallback<T> suggestionsCallback;
  final ItemBuilder<T> itemBuilder;
  final OnSuggestionSelected<T> onSuggestionSelected;
  final bool hideSuggestionsOnKeyboardHide;

  @override
  Widget build(BuildContext context) {
    return TypeAheadField<T>(
      hideSuggestionsOnKeyboardHide: hideSuggestionsOnKeyboardHide,
      debounceDuration: const Duration(milliseconds: 20),
      textFieldConfiguration: TextFieldConfiguration(
        controller: controller,
        decoration: decoration,
      ),
      suggestionsCallback: suggestionsCallback,
      itemBuilder: itemBuilder,
      noItemsFoundBuilder: (context) => const SizedBox(
        height: 100,
        child: Center(
          child: Text(
            'No item found.',
            style: TextStyle(fontSize: 24),
          ),
        ),
      ),
      onSuggestionSelected: onSuggestionSelected,
    );
  }
}
