import 'package:flutter/material.dart';
import '../../models/UserModel.dart';
import '../../services/user_services.dart';
import '../../utils/UserSettings.dart';
import '../auth/login_screen.dart'; 

class SideMenu extends StatefulWidget {
  const SideMenu({super.key});

  @override
  State<SideMenu> createState() => _SideMenuState();
}

class _SideMenuState extends State<SideMenu> {
  late UserModel user;
  void logOut() async {
    UserSettings.setIsLoggedIn(false);
    UserSettings.removeToken();
    UserSettings.setCurrentUser('');

    Navigator.push(
        context, MaterialPageRoute(builder: (context) => const LoginScreen()));
  }

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          FutureBuilder(
              future: UserService.getUser(),
              builder: (context, snapshot) {
                if (snapshot.hasData) {
                  var currentUser = snapshot.data!;
                  return UserAccountsDrawerHeader(
                    accountName: Text(currentUser.name ?? ''),
                    accountEmail: Text(currentUser.emailAddress ?? ''),
                    decoration: const BoxDecoration(
                        color: Colors.blue,
                        image: DecorationImage(
                            image: NetworkImage(
                                'https://thumbs.dreamstime.com/b/green-valley-10835224.jpg'),
                            fit: BoxFit.cover)),
                  );
                }
                return const UserAccountsDrawerHeader(
                  accountName: Text(''),
                  accountEmail: Text(''),
                  decoration: BoxDecoration(
                      color: Colors.blue,
                      image: DecorationImage(
                          image: NetworkImage(
                              'https://thumbs.dreamstime.com/b/green-valley-10835224.jpg'),
                          fit: BoxFit.cover)),
                );
              }),
          GestureDetector(
            child: const ListTile(
              leading: Icon(Icons.person),
              title: Text('Profile'),
            ),
            onTap: () {
              // Navigator.pop(context);
              // Navigator.of(context).push(MaterialPageRoute(
              //     builder: (context) => const ProfileScreen()));
            },
          ),
          GestureDetector(
            child: const ListTile(
              leading: Icon(Icons.people),
              title: Text('Contacts'),
            ),
            onTap: () {
              // Navigator.pop(context);
              // Navigator.of(context).push(MaterialPageRoute(
              //     builder: (context) => const ContactsScreen()));
            },
          ),
          const Divider(
            color: Color.fromARGB(255, 202, 40, 40),
          ),
          ListTile(
              leading: const Icon(Icons.notifications),
              title: const Text('Request'),
              trailing: ClipOval(
                child: Container(
                  color: Colors.redAccent,
                  width: 20,
                  height: 20,
                  child: const Center(
                    child: Text(
                      '8',
                      style: TextStyle(color: Colors.white, fontSize: 12),
                    ),
                  ),
                ),
              )),
          const ListTile(
            leading: Icon(Icons.settings),
            title: Text('Settings'),
          ),
          const Divider(
            color: Color.fromARGB(255, 202, 40, 40),
          ),
          const ListTile(
            leading: Icon(Icons.description),
            title: Text('Policies'),
          ),
          GestureDetector(
            child: const ListTile(
              leading: Icon(Icons.exit_to_app),
              title: Text('Logout'),
            ),
            onTap: () {
              logOut();
            },
          ),
          const Divider(
            color: Color.fromARGB(255, 202, 40, 40),
          ),
        ],
      ),
    );
  }
}
