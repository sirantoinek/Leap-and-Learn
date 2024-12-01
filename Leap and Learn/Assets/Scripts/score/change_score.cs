using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Tracks and updates the player's score based on their frog's movement.
/// Communicates with PlayFab to update the high score if the current score exceeds it.
/// </summary>
public class change_score : MonoBehaviour
{
    /// <summary>
    /// Current score of the player.
    /// </summary>
    public int score;

    /// <summary>
    /// UI element to display the score value.
    /// </summary>
    public TextMeshProUGUI scoreValueTextBox;

    /// <summary>
    /// Reference to the frog game object (player character).
    /// </summary>
    public Transform frog;

    /// <summary>
    /// Tracks the highest Y-position the frog has reached.
    /// Used to calculate score increments.
    /// </summary>
    private float highestYpos;

    /// <summary>
    /// Initializes score, score display, and highest Y-position.
    /// </summary>
    void Start()
    {
        // Initialize variables
        score = 0; // Start the score at 0
        scoreValueTextBox.text = score.ToString();  // Display the initial score
        highestYpos = frog.position.y - 0.1f;   // Set the initial highest Y-position slightly below the frog's position
    }

    /// <summary>
    /// Checks for player movement and updates the score if the frog surpasses the highest Y-position.
    /// </summary>
    void Update()
    {
        // Check if the player (frog) has moved
        if (frog.transform.hasChanged)
        {
            // Check if the frog has moved higher than the previously recorded highest Y-position by at least 1 unit
            if (frog.position.y >= highestYpos + 1)
            {
                // Calculate how many "tiles" the frog has jumped upwards
                int tilesJumped = Mathf.FloorToInt(frog.position.y - highestYpos);

                // Increase the score based on tiles jumped
                increaseScore(tilesJumped);

                // Update the score display
                updateScoreTextBox();

                // Update the highest Y-position to the frog's current position
                highestYpos = frog.position.y;
            }
        }
    }

    /// <summary>
    /// Increases the player's score by the number of tiles jumped.
    /// Updates the PlayFab high score if the current score exceeds it.
    /// </summary>
    /// <param name="tilesJumped">Number of tiles the frog jumped.</param>
    void increaseScore(int tilesJumped)
    {
        // Increment the score by the tiles jumped
        score += tilesJumped;

        // Check and update PlayFab high score
        PlayFabController.Instance.PullHighScore((playfabHighScore) =>
        {
            if (score > playfabHighScore)   // If the current score is higher than the stored high score
            {
                PlayFabController.Instance.UpdateHighScore(score);  // Update the high score in PlayFab
            }
        });
        
    }

    /// <summary>
    /// Updates the score value displayed in the UI.
    /// </summary>
    void updateScoreTextBox()
    {
        scoreValueTextBox.text = score.ToString();  // Update the score text box with the current score
    }
}
