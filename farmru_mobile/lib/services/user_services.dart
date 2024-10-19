 
import '../models/GenericApiResponse.dart';
import '../models/UserModel.dart';
import '../models/authenticate_model.dart';
import '../screens/components/CustomToast.dart';
import '../utils/UserSettings.dart';

import '../utils/base_client.dart';

class UserService {
  static String backendUrl = UserSettings.getBackendUrl();

  static Future<bool> login(String email, String password) async {
    var body = {
      "userNameOrEmailAddress": email,
      "password": password,
    };
    var response = await BaseClient().post('api/TokenAuth/Authenticate', body);
    var apiResponse = genericApiResponseFromJson(response);

    if (apiResponse.success) {
      var authModel = authenticateFromJson(response);
      var token = authModel.result?.accessToken;
      await UserSettings.setToken(token!);
    }
    UserSettings.setIsLoggedIn(apiResponse.success);
    return apiResponse.success;
  }

  static Future<User?> getUser() async {
    UserModel userModel;

    var userFromPref = UserSettings.getCurrentUser();
    if (userFromPref.isNotEmpty) {
      userModel = userModelFromJson(userFromPref);
      return userModel.result?.user;
    }

    var response = await BaseClient()
        .get('api/services/app/Session/GetCurrentLoginInformations');
    if (response != null) {
      userModel = userModelFromJson(response);
      if (userModel.success == true) {
        var user = userModel.result?.user;
        await UserSettings.setCurrentUser(userModelToJson(userModel));
        return user;
      }
    }

    CustomToast.errorShortToast('Failed to get current user');
    return null;
  }
}
