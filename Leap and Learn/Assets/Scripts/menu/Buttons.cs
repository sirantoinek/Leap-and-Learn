using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{

    public GameObject SettingsPage;
    public GameObject AccountInfoPage;
    public GameObject LeaderboardPage;

    public void OpenSettings()
    {
        SettingsPage.SetActive(true);
    }
    public void CloseSettings()
    {
        SettingsPage.SetActive(false);
    }
    public void OpenAccountInfo()
    {
        AccountInfoPage.SetActive(true);
    }
    public void CloseAccountInfo()
    {
        AccountInfoPage.SetActive(false);
    }
    public void OpenLeaderboard()
    {
        LeaderboardPage.SetActive(true);
    }
    public void CloseLeaderboard()
    {
        LeaderboardPage.SetActive(false);
    }
}