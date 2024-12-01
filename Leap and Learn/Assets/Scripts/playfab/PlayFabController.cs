using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.InputManagerEntry;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

/// <summary>
/// Handles PlayFab interactions for user management, leaderboards, statistics, and inventory.
/// This class implements a singleton pattern to ensure a single instance persists across scenes.
/// </summary>
public class PlayFabController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PlayFabController.
    /// </summary>
    public static PlayFabController Instance { get; private set; }

    /// <summary>
    /// Unity's Awake method to initialize the singleton and PlayFab settings.
    /// </summary>
    void Awake()
    {
        // Ensure the PlayFab Title ID is set correctly.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "119EB";  // Set the PlayFab Title ID here
        }

        // Enforce singleton pattern.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);    // Persist across scenes.
        }
    }

    #region ConfigDefaultData

    /// <summary>
    /// Configures default data for a new user (e.g., high score, inventory placeholders).
    /// </summary>
    public void FirstTimeConfig()
    {
        UpdateHighScore(0); // Initialize high score to 0.
        SetHats(new Dictionary<string, bool> { { "EmptyHat", true } }); // Placeholder for hats.
        SetSkins(new Dictionary<string, bool> { { "EmptySkins", true } });  // Placeholder for skins.
        SetCoins(0);    // Initialize coins to 0.
    }

    /// <summary>
    /// Actions performed during login, such as retrieving player data.
    /// </summary>
    public void OnLogin(Action action)
    {
        PullHighScore((tempHighScore) => { });  // Fetch player's high score.
        GetAllPlayerData(); // Retrieve all player-related data from PlayFab.
        action.Invoke();
    }

    /// <summary>
    /// Clears all locally stored user variables.
    /// </summary>
    private void ClearAllVars()
    {
        username = null;
        playFabID = null;
        coins = 0;
        hats = null;
        skins = null;
        highScore = 0;
        currentHat = null;
        currentSkin = "Green";
    }

    #endregion ConfigDefaultData

    #region Login
    private string username;    // Player's username.
    private string playFabID;   // PlayFab unique ID for the player.

    /// <summary>
    /// Gets the player's username.
    /// </summary>
    /// <returns>Player's username.</returns>
    public string GetUsername() => username;

    /// <summary>
    /// Gets the player's PlayFab ID.
    /// </summary>
    /// <returns>Player's PlayFab ID.</returns>
    public string GetPlayFabID() => playFabID;

    /// <summary>
    /// Sets the player's username.
    /// </summary>
    /// <param name="username">New username to assign.</param>
    public void SetUsername(string username) => this.username = username;

    /// <summary>
    /// Sets the player's PlayFab ID.
    /// </summary>
    /// <param name="playFabID">New PlayFab ID to assign.</param>
    public void SetPlayFabID(string playFabID) => this.playFabID = playFabID;

    /// <summary>
    /// Logs out the player, clears all credentials, and returns to the main menu.
    /// </summary>
    public void Logout()
    {
        if (playFabID != null)
        {
            PlayFabClientAPI.ForgetAllCredentials();
            ClearAllVars();
            Debug.Log("Logged out and cleared variables");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("You are not currently logged in");
        }
    }

    #endregion Login

    #region PlayerStats

    /// <summary>
    /// Callback for a successful statistic update.
    /// </summary>
    /// <param name="result">Result of the update operation.</param>
    private void OnStatisticUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"Successful: Statistics Sent!");
    }

    /// <summary>
    /// Callback for errors during statistic operations.
    /// </summary>
    /// <param name="error">Error information from PlayFab.</param>
    private void OnStatisticError(PlayFabError error)
    {
        Debug.LogError($"Statistic Error: {error.GenerateErrorReport()}");
    }

    #region HighScore
    public int highScore = 0;   // Player's high score.

    /// <summary>
    /// Fetches the player's high score from PlayFab.
    /// </summary>
    /// <param name="callback">Action to execute with the retrieved score.</param>
    public void PullHighScore(Action<int> callback)
    {
        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string>()
            {
                "PlayerHighScore"
            }
        };
        PlayFabClientAPI.GetPlayerStatistics(request, (result) =>
        {
            highScore = result.Statistics.First().Value;
            callback?.Invoke(highScore);
        }, OnStatisticError);
    }

    /// <summary>
    /// Updates the player's high score on PlayFab.
    /// </summary>
    /// <param name="score">New high score value.</param>
    public void UpdateHighScore(int score)
    {
        highScore = score;
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {
                    StatisticName = "PlayerHighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatisticUpdate, OnStatisticError);
        Debug.Log($"Updating high score for user {username}: {score}");
    }

    #endregion HighScore

    #region LeaderBoard
    public List<PlayerLeaderboardEntry> leaderboardEntries = new List<PlayerLeaderboardEntry>();

    /// <summary>
    /// Fetches the top 10 leaderboard entries and the player's leaderboard position.
    /// </summary>
    /// <returns>List of leaderboard entries.</returns>
    public List<PlayerLeaderboardEntry> GetLeaderboardEntries(Action action)
    {
        GetTop10Leaderboard((top10) => // Initiates the API call 
        {
            leaderboardEntries = top10;
            GetAroundPlayerLeaderboard((playerLeaderBoardPos) => // Initiates another API call
            {
                if (playerLeaderBoardPos != null)
                {
                    if (!leaderboardEntries.Any(entry => entry.DisplayName == playerLeaderBoardPos.DisplayName))
                    {
                        if (leaderboardEntries.LastOrDefault().StatValue == playerLeaderBoardPos.StatValue)
                        {
                            playerLeaderBoardPos.Position = leaderboardEntries.LastOrDefault().Position + 1;
                        }
                        leaderboardEntries.Add(playerLeaderBoardPos);
                        Debug.LogError($"User in postion {playerLeaderBoardPos.Position}");
                    }
                }
                else
                {
                    Debug.LogError("NULL leaderboard!!!");
                }
                action.Invoke();
            });
        });

        return leaderboardEntries;
    }

    /// <summary>
    /// Fetches the top 10 players from the leaderboard.
    /// </summary>
    private void GetTop10Leaderboard(Action<List<PlayerLeaderboardEntry>> callback)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerHighScore",
            StartPosition = 0,
            MaxResultsCount = 10  // Get top 10 results
        };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            callback?.Invoke(result.Leaderboard);
        }, OnLeaderboardError);
    }

    /// <summary>
    /// Fetches the player's position in the leaderboard.
    /// </summary>
    private void GetAroundPlayerLeaderboard(Action<PlayerLeaderboardEntry> callback)
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "PlayerHighScore",
            MaxResultsCount = 1,    // Get Player's postion in the leaderboard
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, (result) =>
        {
            callback?.Invoke(result.Leaderboard.FirstOrDefault());
        }, OnLeaderboardError);
    }

    /// <summary>
    /// Callback for errors during leaderboard operations.
    /// </summary>

    private void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Leaderboard Error: {error.GenerateErrorReport()}");
    }


    #endregion Leaderboard


    #endregion PlayerStats

    #region PlayerData

    // Stores the player's total coins.
    private int coins;

    // Stores the player's unlocked hats as a dictionary.
    // The key is the hat name, and the value is a boolean indicating ownership.
    private Dictionary<string, bool> hats;

    // Stores the player's unlocked skins as a dictionary.
    // The key is the skin name, and the value is a boolean indicating ownership.
    private Dictionary<string, bool> skins;

    // Stores the player's currently equipped skin. Default is "Green".
    private string currentSkin = "Green";

    // Stores the player's currently equipped hat.
    private string currentHat;

    /// <summary>
    /// Updates the player's coin count.
    /// </summary>
    /// <param name="coins">The new coin count to set.</param>
    public void SetCoins(int coins)
    {
        SetCoinData(coins);
    }

    /// <summary>
    /// Updates the player's unlocked hats.
    /// </summary>
    /// <param name="tempHats">A dictionary of hats with their ownership status.</param>
    public void SetHats(Dictionary<string, bool> tempHats)
    {
        SetHatsData(tempHats);
    }

    /// <summary>
    /// Updates the player's unlocked skins.
    /// </summary>
    /// <param name="tempSkins">A dictionary of skins with their ownership status.</param>
    public void SetSkins(Dictionary<string, bool> tempSkins)
    {
        SetSkinsData(tempSkins);
    }

    /// <summary>
    /// Sets the currently equipped skin.
    /// </summary>
    /// <param name="newCurrentSkin">The name of the new skin to equip.</param>
    public void SetCurrentSkin(string newCurrentSkin)
    {
        currentSkin = newCurrentSkin;
    }

    /// <summary>
    /// Gets the name of the currently equipped skin.
    /// </summary>
    /// <returns>The currently equipped skin.</returns>
    public string GetCurrentSkin()
    {
        return currentSkin;
    }

    /// <summary>
    /// Sets the currently equipped hat.
    /// </summary>
    /// <param name="newCurrentHat">The name of the new hat to equip.</param>
    public void SetCurrentHat(string newCurrentHat)
    {
        currentHat = newCurrentHat;
    }

    /// <summary>
    /// Gets the name of the currently equipped hat.
    /// </summary>
    /// <returns>The currently equipped hat.</returns>
    public string GetCurrentHat()
    {
        return currentHat;
    }

    /// <summary>
    /// Retrieves all player data, including coins, skins, and hats, from PlayFab.
    /// </summary>
    private void GetAllPlayerData()
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = null
        };
        PlayFabClientAPI.GetUserData(request, OnInventoryGet, OnInventoryError);
    }

    /// <summary>
    /// Fetches the player's coin data from PlayFab.
    /// </summary>
    /// <param name="callback">Callback to handle the retrieved coin count.</param>
    public void GetPlayerCoinData(Action<int> callback)
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Coins" }
        };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            if (result.Data.ContainsKey("Coins"))
            {
                int.TryParse(result.Data["Coins"].Value, out coins);
                Debug.Log($"{username} has {coins} coins");
            }
            callback?.Invoke(coins);
        }, OnInventoryError);
    }

    /// <summary>
    /// Fetches the player's skin data from PlayFab.
    /// </summary>
    /// <param name="callback">Callback to handle the retrieved skin data.</param>
    public void GetPlayerSkinData(Action<Dictionary<string, bool>> callback)
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Skins" }
        };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            skins = new Dictionary<string, bool>();
            if (result.Data != null && result.Data.ContainsKey("Skins"))
            {
                string skinsJson = result.Data["Skins"].Value;
                skins = JsonConvert.DeserializeObject<Dictionary<string, bool>>(skinsJson);
            }
            callback?.Invoke(skins);
        }, OnInventoryError);
    }

    /// <summary>
    /// Fetches the player's hat data from PlayFab.
    /// </summary>
    /// <param name="callback">Callback to handle the retrieved hat data.</param>
    public void GetPlayerHatsData(Action<Dictionary<string, bool>> callback)
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Hats" }
        };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            hats = new Dictionary<string, bool>();
            if (result.Data != null && result.Data.ContainsKey("Hats"))
            {
                string hatsJson = result.Data["Hats"].Value;
                hats = JsonConvert.DeserializeObject<Dictionary<string, bool>>(hatsJson);
            }
            callback?.Invoke(hats);
        }, OnInventoryError);
    }

    /// <summary>
    /// Updates the player's coin count on PlayFab.
    /// </summary>
    /// <param name="newCoins">The new coin count to set.</param>
    private void SetCoinData(int newCoins)
    {
        coins = newCoins;
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Coins", coins.ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnInventorySet, OnInventoryError);
    }

    /// <summary>
    /// Updates the player's skin data on PlayFab.
    /// </summary>
    /// <param name="newSkins">A dictionary of skins with their ownership status.</param>
    private void SetSkinsData(Dictionary<string, bool> newSkins)
    {
        skins = newSkins;
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Skins", JsonConvert.SerializeObject(skins) }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnInventorySet, OnInventoryError);
    }

    /// <summary>
    /// Updates the player's hat data on PlayFab.
    /// </summary>
    /// <param name="newHats">A dictionary of hats with their ownership status.</param>
    private void SetHatsData(Dictionary<string, bool> newHats)
    {
        hats = newHats;
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Hats", JsonConvert.SerializeObject(hats) }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnInventorySet, OnInventoryError);
    }

    /// <summary>
    /// Callback for successful retrieval of inventory data from PlayFab.
    /// Logs details about the retrieved inventory.
    /// </summary>
    /// <param name="result">The result containing user data.</param>
    private void OnInventoryGet(GetUserDataResult result)
    {
        Debug.Log("Got inventory");
        if (result.Data != null)
        {
            if (result.Data.ContainsKey("Coins"))
            {
                int.TryParse(result.Data["Coins"].Value, out coins);
                Debug.Log($"{username} has {coins} coins");
            }
            if (result.Data.ContainsKey("Skins"))
            {
                string skinsJson = result.Data["Skins"].Value;
                skins = JsonConvert.DeserializeObject<Dictionary<string, bool>>(skinsJson);
                Debug.Log($"{username} has {skins.Count} skins");
            }
            if (result.Data.ContainsKey("Hats"))
            {
                string hatsJson = result.Data["Hats"].Value;
                hats = JsonConvert.DeserializeObject<Dictionary<string, bool>>(hatsJson);
                Debug.Log($"{username} has {hats.Count} hats");
            }
        }
    }

    /// <summary>
    /// Callback for successfully updating inventory data on PlayFab.
    /// </summary>
    /// <param name="result">Result of the update operation.</param>
    private void OnInventorySet(UpdateUserDataResult result)
    {
        Debug.Log("Successfully modified to inventory");
    }

    /// <summary>
    /// Callback for errors encountered during inventory operations.
    /// Logs the error details.
    /// </summary>
    /// <param name="error">The error details from PlayFab.</param>
    private void OnInventoryError(PlayFabError error)
    {
        Debug.LogError($"Inventory Error: {error.GenerateErrorReport()}");
    }

    #endregion PlayerData
}
