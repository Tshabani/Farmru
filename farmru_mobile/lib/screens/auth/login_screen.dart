// import 'package:farm_app/screens/home_screen.dart';
// ignore_for_file: use_build_context_synchronously

import 'package:flutter/material.dart';

import '../../services/user_services.dart';
import '../../utils/UserSettings.dart';
import '../components/CustomToast.dart';
import '../home/home_screen.dart';

var isLoggedIn = false;

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  // form key
  final _formKey = GlobalKey<FormState>();
  bool _isLoading = false;

  //editing controller
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  void login() async {
    var email = emailController.text;
    var password = passwordController.text;

    if (email.isEmpty) {
      CustomToast.showCenterShortToast("Please email or username");
      return;
    }

    if (password.isEmpty || password.length < 6) {
      CustomToast.showCenterShortToast(
          "Please password or make sure password has 6 characters");
      return;
    }

    setState(() {
      _isLoading = true;
    });

    if (await UserService.login(email, password)) {
      setState(() {
        _isLoading = false;
        isLoggedIn = true;
      });
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => const HomeScreen(),
        ),
      );
    } else {
      setState(() {
        _isLoading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    isLoggedIn = UserSettings.getIsLoggedIn();
    UserSettings.setIsLoggedIn(isLoggedIn);
  }

  @override
  Widget build(BuildContext context) {
    //email field
    final emailField = TextFormField(
      autofocus: false,
      controller: emailController,
      keyboardType: TextInputType.emailAddress,
      onSaved: (value) {
        emailController.text = value!;
      },
      textInputAction: TextInputAction.next,
      decoration: InputDecoration(
          prefixIcon: const Icon(Icons.mail),
          contentPadding: const EdgeInsets.fromLTRB(20, 15, 20, 15),
          hintText: "Email",
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
          )),
    );

    final passwordField = TextFormField(
      autofocus: false,
      controller: passwordController,
      obscureText: true,
      onSaved: (value) {
        passwordController.text = value!;
      },
      textInputAction: TextInputAction.done,
      decoration: InputDecoration(
          prefixIcon: const Icon(Icons.key),
          contentPadding: const EdgeInsets.fromLTRB(20, 15, 20, 15),
          hintText: "Password",
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
          )),
    );

    return Scaffold(
      backgroundColor: Colors.white,
      body: isLoggedIn
          ? const HomeScreen()
          : Container(
              child: _isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Center(
                        child: SingleChildScrollView(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: [
                              Form(
                                key: _formKey,
                                child: Column(
                                  children: <Widget>[
                                    Image.asset(
                                      'assets/FARMRU_no_bg.png', // replace with your image asset
                                      fit: BoxFit.cover,
                                    ),
                                    const SizedBox(
                                      height: 15,
                                    ),
                                    emailField,
                                    const SizedBox(
                                      height: 15,
                                    ),
                                    passwordField,
                                    const SizedBox(
                                      height: 15,
                                    ),
                                    ElevatedButton(
                                      onPressed: () async {
                                        login();
                                      },
                                      style: ElevatedButton.styleFrom(
                                          minimumSize:
                                              const Size(double.infinity, 35),
                                          shape: RoundedRectangleBorder(
                                            borderRadius:
                                                BorderRadius.circular(30.0),
                                          ),
                                          backgroundColor:
                                              const Color(0xFFB7873B)),
                                      child: const Text(
                                        'Login',
                                        style:
                                            TextStyle(color: Color(0xFFFFFFFF)),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
            ),
    );
  }
}
