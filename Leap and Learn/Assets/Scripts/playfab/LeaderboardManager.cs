using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;
using System.Collections;

public class LeaderboardManager : MonoBehaviour
{
    private List<PlayerLeaderboardEntry> leaderboardEntries;

    private void Start()
    {
        // StartCoroutine(GetLeaderboardCoroutine());
    }

    private IEnumerator GetLeaderboardCoroutine()
    {
        while (true)
        {
            GetLeaderboard();
            yield return new WaitForSeconds(5); // Poll Leaderboard every 5sec
        }
    }

    public void GetLeaderboard()
    {
        leaderboardEntries = PlayFabController.Instance.GetLeaderboardEntries();

        // Do something with leaderboardEntries
        // if user is not in top 10 .. user will be the 11th entry (the last)
        // 11th entry not operational yet....

        foreach (var item in leaderboardEntries)
        {
            Debug.Log(string.Format("PLACE: {0} | NAME: {1} | HIGHSCORE: {2}",
                item.Position, item.DisplayName, item.StatValue));
        }
    }
}
