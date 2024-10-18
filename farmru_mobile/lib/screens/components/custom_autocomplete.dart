// ignore_for_file: file_names

import 'package:flutter/material.dart';
import 'package:flutter_typeahead/flutter_typeahead.dart';

import '../../models/EntityWithDisplayNameDto.dart';
import '../../services/autocomplete_services.dart';

class CustomAutocomplete extends StatefulWidget {
  final EntityWithDisplayNameDto vehicle;
  const CustomAutocomplete({super.key, required this.vehicle});

  @override
  State<CustomAutocomplete> createState() => CustomAutocompleteState();
}

class CustomAutocompleteState extends State<CustomAutocomplete> {
  final vehicleController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return TypeAheadField<EntityWithDisplayNameDto?>(
        hideSuggestionsOnKeyboardHide: false,
        debounceDuration: const Duration(milliseconds: 20),
        textFieldConfiguration: TextFieldConfiguration(
          controller: vehicleController,
          decoration: const InputDecoration(
            // prefixIcon: Icon(Icons.search),
            border: OutlineInputBorder(),
            labelText: 'Vehicle',
            hintText: 'Search vehicle by number plate',
          ),
        ),
        suggestionsCallback: AutocompleteService.vehicleAutoComplete,
        itemBuilder: (context, EntityWithDisplayNameDto? suggestion) {
          final vehicle = suggestion!;
          return ListTile(
            title: Text(vehicle.displayName ?? ""),
          );
        },
        // ignore: sized_box_for_whitespace
        noItemsFoundBuilder: (context) => Container(
              height: 100,
              child: const Center(
                child: Text(
                  'No vehicles found.',
                  style: TextStyle(fontSize: 24),
                ),
              ),
            ),
        onSuggestionSelected: (EntityWithDisplayNameDto? suggestion) {
          vehicleController.text = suggestion!.displayName!;
        });
  }
}

