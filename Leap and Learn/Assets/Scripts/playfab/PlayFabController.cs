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

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController Instance { get; private set; }

    void Awake()
    {
        Debug.Log("awake test");
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "119EB";
        }

        if (Instance != null && Instance != this)
        {
            Debug.Log("AWAKE DELETE");
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    #region ConfigDefaultData
    public void FirstTimeConfig()
    {
        UpdateHighScore(0);
        // Just making placeholders for new user
        SetHats(new Dictionary<string, bool> { { "EmptyHat", true } }); 
        SetSkins(new Dictionary<string, bool> { { "EmptySkins", true } });
        SetCoins(0);
    }

    public void OnLogin()
    {
        PullHighScore();
        GetAllPlayerData();
    }

    private void ClearAllVars()
    {
        username = null;
        playFabID = null;
        coins = 0;
        hats = null;
        skins = null;
        highScore = 0;
    }

    #endregion ConfigDefaultData

    #region Login
    private string username;
    private string playFabID;
    public string GetUsername()
    {
        return username;
    }

    public string GetPlayFabID()
    {
        return playFabID;
    }

    public void SetUsername(string username)
    {
        this.username = username;
    }

    public void SetPlayFabID(string playFabID)
    {
        this.playFabID = playFabID;
    }

    public void Logout()
    {
        Debug.Log("LOGOUT REACHED");
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
    private void OnStatisticUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"Successful: Statistics Sent!");
    }

    private void OnStatisticError(PlayFabError error)
    {
        Debug.LogError($"Statistic Error: {error.GenerateErrorReport()}");
    }

    #region HighScore
    public int highScore = 0;

    private void PullHighScore()
    {
        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string>()
            {
                "PlayerHighScore"
            }
        };

        PlayFabClientAPI.GetPlayerStatistics(request, OnStatisticGet, OnStatisticError);
    }

    public int GetHighScore()
    {
        PullHighScore();
        return highScore;
    }

    private void OnStatisticGet(GetPlayerStatisticsResult result)
    {
        highScore = result.Statistics.First().Value;
    }


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

    public List<PlayerLeaderboardEntry> GetLeaderboardEntries()
    {
        GetTop10Leaderboard(); // Initiates the API call
        GetAroundPlayerLeaderboard(); // Initiates another API call

        return leaderboardEntries;
    }

    private void GetTop10Leaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerHighScore",
            StartPosition = 0,
            MaxResultsCount = 10  // Get top 10 results
        };
        PlayFabClientAPI.GetLeaderboard(request, OnTop10LeaderboardGet, OnLeaderboardError);
    }

    private void GetAroundPlayerLeaderboard()
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "PlayerHighScore",
            MaxResultsCount = 1,
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnPlayerLeaderboardGet, OnLeaderboardError);
    }

    private void OnTop10LeaderboardGet(GetLeaderboardResult result)
    {
        if (result.Leaderboard != null && result.Leaderboard.Count > 0)
        {
            Debug.Log($"Leaderboard successfully fetched. Top {result.Leaderboard.Count} players:");
            foreach (var entry in result.Leaderboard)
            {
                Debug.Log($"Player: {entry.DisplayName}, Score: {entry.StatValue}");
            }
            leaderboardEntries = result.Leaderboard;
        }
        else
        {
            Debug.LogWarning("Leaderboard fetched, but no entries found.");
        }
    }

    // Currently not operational
    private void OnPlayerLeaderboardGet(GetLeaderboardAroundPlayerResult result)
    {
        foreach (var currentPlayer in result.Leaderboard)
        {
            if (!leaderboardEntries.Contains(currentPlayer))
            {
                leaderboardEntries.Append(currentPlayer);
            }
            else
            {
                Debug.Log($"{username} is top 10! Current place is {currentPlayer.Position}");
            }
        }
    }

    private void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Leaderboard Error: {error.GenerateErrorReport()}");
    }


    #endregion Leaderboard


    #endregion PlayerStats

    #region PlayerData

    private int coins;
    private Dictionary<string, bool> hats;
    private Dictionary<string, bool> skins;

    public int GetCoins()
    {
        GetPlayerCoinData();
        return coins;
    }

    public void SetCoins(int coins)
    {
        SetCoinData(coins);
    }

    public Dictionary<string, bool> GetHats()
    {
        GetPlayerHatsData();
        return hats;
    }

    public void SetHats(Dictionary<string, bool> tempHats)
    {
        SetHatsData(tempHats);
    }

    public Dictionary<string, bool> GetSkins()
    {
        GetPlayerSkinData();
        return skins;
    }

    public void SetSkins(Dictionary<string, bool> tempSkins)
    {
        SetSkinsData(tempSkins);
    }


    private void GetAllPlayerData()
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = null
        };
        PlayFabClientAPI.GetUserData(request, OnInventoryGet, OnInventoryError);
    }

    private void GetPlayerCoinData()
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Coins" }
        };
        PlayFabClientAPI.GetUserData(request, OnInventoryGet, OnInventoryError);
    }

    private void GetPlayerSkinData()
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Skins" }
        };
        PlayFabClientAPI.GetUserData(request, OnInventoryGet, OnInventoryError);
    }

    private void GetPlayerHatsData()
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = playFabID,
            Keys = new List<string> { "Hats" }
        };
        PlayFabClientAPI.GetUserData(request, OnInventoryGet, OnInventoryError);
    }

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
                string skinsJson = result.Data["Hats"].Value;
                hats = JsonConvert.DeserializeObject<Dictionary<string, bool>>(skinsJson);
                Debug.Log($"{username} has {hats.Count} hats");
            }
        }
    }

    private void OnInventorySet(UpdateUserDataResult result)
    {
        Debug.Log("Successfully modified to inventory");
    }

    private void OnInventoryError(PlayFabError error)
    {
        Debug.LogError($"Inventory Error: {error.GenerateErrorReport()}");
    }

    #endregion PlayerData
}
