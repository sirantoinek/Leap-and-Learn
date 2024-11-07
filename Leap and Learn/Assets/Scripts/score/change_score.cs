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
        highestYpos = frog.position.y - 1;
    }


    void Update()
    {
        // Check if the player pressed the "W" or "Up Arrow" key
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Check if the frog's current y-pos is higher than the previously recorded highest y-pos
            if (frog.position.y > highestYpos)
            {
                increaseScore();
                updateScoreTextBox();
                highestYpos = frog.position.y;
            }
        }
    }

    // Increment the score by +1
    void increaseScore()
    {
        score++;
    }

    // Update the score in the textbox
    void updateScoreTextBox()
    {
        scoreValueTextBox.text = score.ToString();
    }
}
