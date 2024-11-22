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
    private frogMovement movement;
    

    void Start()
    {
        currentHearts = maxHearts;
        movement = GetComponent<frogMovement>();
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

    public void GainHeart()
    {
        if (currentHearts < 3)
        {
            currentHearts++;
            UpdateHeartsUI();
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
        movement.lockMovement();
    }
}