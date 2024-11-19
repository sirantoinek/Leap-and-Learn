using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userNameTextBox;
    public TMP_InputField passwordTextBox;
    public TextMeshProUGUI loginErrorTextBox;
    public GameObject LoginPanel;

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(userNameTextBox.text) || string.IsNullOrEmpty(passwordTextBox.text))
        {
            Debug.LogError("Username or password cannot be empty!");
            return;
        }

        string username = userNameTextBox.text;
        string password = passwordTextBox.text;

        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"Login successful! PlayFab ID: {result.PlayFabId}");

        PlayFabController.Instance.SetUsername(userNameTextBox.text);
        PlayFabController.Instance.SetPlayFabID(result.PlayFabId);
        PlayFabController.Instance.OnLogin();

        LoginPanel.SetActive(false);    // hides the panel when login success.
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"Login failed: {error.GenerateErrorReport()}");

        string errorMesssage = error.GenerateErrorReport();

        loginErrorTextBox.text = ("Login failed: Invalid username or password.");


        //loginErrorTextBox.text = ($"Login failed: {error.GenerateErrorReport()}");

    }

    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(userNameTextBox.text) || string.IsNullOrEmpty(passwordTextBox.text))
        {
            Debug.LogError("Username or password cannot be empty!");
            return;
        }

        string username = userNameTextBox.text;
        string password = passwordTextBox.text;

        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false,
            DisplayName = username
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log($"Registration successful! PlayFab ID: {result.PlayFabId}");

        PlayFabController.Instance.SetUsername(userNameTextBox.text);
        PlayFabController.Instance.SetPlayFabID(result.PlayFabId);
        PlayFabController.Instance.FirstTimeConfig(); // Setup the user
        LoginPanel.SetActive(false);    // hides the panel when login success.
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError($"Registration failed: {error.GenerateErrorReport()}");

        string errorMesssage = error.GenerateErrorReport();

        loginErrorTextBox.text = "Registration failed: ";

        switch (error.Error.ToString())
        {
            case "InvalidUsername":
                loginErrorTextBox.text += "Invalid Username";
                break;
            case "ProfaneDisplayName":
                loginErrorTextBox.text += "Invalid Username";
                break;
            case "UsernameNotAvailable":
                loginErrorTextBox.text += "Username in use...\nEnter another";
                break;
            case "NameNotAvailable":
                loginErrorTextBox.text += "Username in use...\nEnter another";
                break;
            default:
                loginErrorTextBox.text += ($"{error.GenerateErrorReport()}");
                break;
        }

    }
}
