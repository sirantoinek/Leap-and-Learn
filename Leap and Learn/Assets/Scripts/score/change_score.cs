using System;
using TMPro;
using UnityEngine;

public class change_score : MonoBehaviour
{
    public int score;

    public TextMeshProUGUI scoreValueTextBox;

    public Transform frog;

    private float highestYpos;

    void Start()
    {
        // initialize vars
        score = 0;
        scoreValueTextBox.text = score.ToString();
        highestYpos = frog.position.y - 0.1f;
    }

    void Update()
    {
        // Check if the player has moved
        if (frog.transform.hasChanged)
        {
            // Check if the frog's current y-pos is higher than the previously recorded highest y-pos
            if (frog.position.y >= highestYpos + 1)
            {
                int tilesJumped = Mathf.FloorToInt(frog.position.y - highestYpos);
                increaseScore(tilesJumped);
                updateScoreTextBox();
                highestYpos = frog.position.y;
            }
        }
    }

    // Increment the score by tiles jumped
    void increaseScore(int tilesJumped)
    {
        score += tilesJumped;
    }

    // Update the score in the textbox
    void updateScoreTextBox()
    {
        scoreValueTextBox.text = score.ToString();
    }
}
