import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/material.dart';
import '../../models/category_model.dart';
import '../Nodes/NodeDetails.dart';
import '../components/CustomContainer.dart';
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
                CustomContainer(
                  text: 'Facility 1',
                  SubText: '91000001',
                  page: NodeDetailsPage(),
                ),
                CustomContainer(
                  text: 'Facility 1',
                  SubText: '91000002',
                  page: NodeDetailsPage(),
                ),
                CustomContainer(
                  text: 'Facility 1',
                  SubText: '91000003',
                  page: NodeDetailsPage(),
                ),
                CustomContainer(
                  text: 'Facility 1',
                  SubText: '91000004',
                  page: NodeDetailsPage(),
                ),
                CustomContainer(
                  text: 'Facility 1',
                  SubText: '91000005',
                  page: NodeDetailsPage(),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
