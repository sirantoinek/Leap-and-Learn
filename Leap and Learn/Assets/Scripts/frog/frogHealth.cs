using UnityEngine;
using UnityEngine.UI;

public class frogHealth : MonoBehaviour
{
    // Array to hold the heart images
    public Image[] hearts; 
    public int maxHearts = 3;
    public int currentHearts;

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
                hearts[i].enabled = true;  // Show heart
            }
            else
            {
                hearts[i].enabled = false; // Hide heart
            }
        }
    }

    void GameOver()
    {
        // Do whatever when gameover
    }
}