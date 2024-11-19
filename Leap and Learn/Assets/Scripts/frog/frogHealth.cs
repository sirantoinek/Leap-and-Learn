using UnityEngine;
using UnityEngine.UI;

public class frogHealth : MonoBehaviour
{
    public Sprite heartOn;
    public Sprite heartOff;
    // Array to hold the heart images
    public Image[] hearts; 
    public int maxHearts = 3;
    public int currentHearts;
    public GameObject GameOverScreen;

    void Start()
    {
        currentHearts = maxHearts;
        UpdateHeartsUI();
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            UpdateHeartsUI();
        }

        if (currentHearts <= 0)
        {
            GameOver();
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHearts)
            {
                hearts[i].sprite = heartOn;  // Heart is "enabled"
            }
            else
            {
                hearts[i].sprite = heartOff; // Heart is "disabled"
            }
        }
    }

    void GameOver()
    {
        GameOverScreen.SetActive(true);
    }
}