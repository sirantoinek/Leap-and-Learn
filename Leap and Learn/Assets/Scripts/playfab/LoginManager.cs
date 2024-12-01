using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userNameTextBox;
    public TMP_InputField passwordTextBox;
    public TextMeshProUGUI loginErrorTextBox;
    public GameObject LoginPanel;

    public TextMeshProUGUI AccountInfoUsername;
    public TextMeshProUGUI AccountInfoHighScore;
    public TextMeshProUGUI AccountInfoCoinCount;
    public TextMeshProUGUI UILeaderboard;

    private void Start()
    {
        if (PlayFabController.Instance.GetPlayFabID() != null)
        {
            LoginPanel.SetActive(false);
            PopulateAccountInfo();
            PopulateUILeaderboard();
        }
        else LoginPanel.SetActive(true);
    }

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

    private void PopulateAccountInfo() // populate the account info page
    {
        //Invoke("PopulateAccountInfoHelper", 2.0f); // Delay in seconds before PopulateAccountInfoHelper is called (to give API time to respond)
        PopulateAccountInfoHelper();

    }

    private void PopulateAccountInfoHelper() // populate the account info page
    {
        AccountInfoUsername.text = ("Username: " + PlayFabController.Instance.GetUsername());
        PlayFabController.Instance.PullHighScore((tempHighScore) =>
        {
            AccountInfoHighScore.text = ("Highscore: " + tempHighScore);
        });
        PlayFabController.Instance.GetPlayerCoinData((tempCoins) =>
        {
            AccountInfoCoinCount.text = ("Coins: " + tempCoins);
        });

    }

    private void PopulateUILeaderboard()
    {
        // Populate leaderboard entries
        PlayFabController.Instance.GetLeaderboardEntries(() =>
        {
            PopulateUILeaderboardHelper();
        });
    }

    private void PopulateUILeaderboardHelper()
    {
        // Check if entries are fetched (if empty, we can wait for the data to arrive)
        if (PlayFabController.Instance.leaderboardEntries == null || PlayFabController.Instance.leaderboardEntries.Count == 0)
        {
            UILeaderboard.text = ("No leaderboard entries available.");
            return;
        }

        UILeaderboard.text = ""; // Clearing Leaderboard

        // Iterate through the leaderboard entries and update the UI
        foreach (var item in PlayFabController.Instance.leaderboardEntries)
        {
            UILeaderboard.text += string.Format("{0}. {1,-15} {2,-6}\n",
                item.Position + 1, item.DisplayName, item.StatValue);
        }
    }


    public void LogoutButton()
    {
        PlayFabController.Instance.Logout();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"Login successful! PlayFab ID: {result.PlayFabId}");

        PlayFabController.Instance.SetUsername(userNameTextBox.text);
        PlayFabController.Instance.SetPlayFabID(result.PlayFabId);
        PlayFabController.Instance.OnLogin(() =>
        {
            PopulateAccountInfo();
            PopulateUILeaderboard();
        });

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
