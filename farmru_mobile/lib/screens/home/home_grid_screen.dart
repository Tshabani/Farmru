import 'package:carousel_slider/carousel_slider.dart'; 
import 'package:flutter/material.dart'; 
import '../../models/category_model.dart';
import '../components/CustomContainer .dart';
import '../components/carousel_card.dart'; 
import 'side_menu.dart';

class HomeGridScreen extends StatefulWidget {
  const HomeGridScreen({super.key});

  @override
  State<HomeGridScreen> createState() => _HomeGridScreenState();
}

class _HomeGridScreenState extends State<HomeGridScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const SideMenu(),
      appBar: AppBar(
        title: const Text(
          'Home',
        ),
      ),
      body: Column(
        children: [
          CarouselSlider(
            options: CarouselOptions(
              aspectRatio: 2.0,
              viewportFraction: 0.9,
              enlargeCenterPage: true,
              enlargeStrategy: CenterPageEnlargeStrategy.height,
              height: 250,
            ),
            items: Category.categories
                .map((category) => CarouselCard(category: category))
                .toList(),
          ),
          Padding(
            padding: const EdgeInsets.all(10.0),
            child: GridView(
              scrollDirection: Axis.vertical,
              shrinkWrap: true,
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 4, mainAxisSpacing: 20, crossAxisSpacing: 20),
              children: const [
                // CustomContainer(
                //   icon: Icons.book_online,
                //   text: 'Booking',
                //   page: BookingListPage(),
                // ),
                // CustomContainer(
                //   icon: Icons.drive_eta_outlined,
                //   text: 'Trip',
                //   page: LogSheetListPage(),
                // ),
                // CustomContainer(
                //   icon: Icons.contact_mail_outlined,
                //   text: 'Contact',
                //   page: ContactsScreen(),
                // ),
                // CustomContainer(
                //   icon: Icons.inventory_2_outlined,
                //   text: 'Inventory',
                //   page: FacilityListPage(),
                // ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
