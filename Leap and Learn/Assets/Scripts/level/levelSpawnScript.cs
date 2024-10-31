using UnityEngine;
using System.Collections.Generic;

public class levelSpawnScript : MonoBehaviour
{

    public Transform frog;
    // Randomization can be added by creating different levels and selecting a random one to spawn (levels should be 10 units in height)
    public GameObject level1;

    Vector3 lastPos; // Used to store previous spawn position
    Queue<GameObject> levels = new Queue<GameObject>(); // Queue to hold levels that are currently on screen

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializing the starting area
        lastPos = spawnLevel(-25, level1);
        lastPos = spawnLevel((int)lastPos.y, level1);
        lastPos = spawnLevel((int)lastPos.y, level1);
    }

    // Update is called once per frame
    void Update()
    {
        // maxViewDistance is the furthest point a player can be before they can see off the edge of the "level" with the camera
        int maxViewDistance = (int)lastPos.y - 2; // -2 by default as the player can see 2 tiles behind the frog
        if (frog.position.y >= maxViewDistance)
        {
            // Spawn a new level and delete the oldest level that is now offscreen
            lastPos = spawnLevel((int)lastPos.y, level1);
            deleteLevel();
        }
    }

    Vector3 spawnLevel(int yOffset, GameObject desiredLevel)
    {
        // Offset the stationary levelSpawner position with yOffset to place new levels in the correct location
        Vector3 desiredPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        GameObject newLevel = Instantiate(desiredLevel, desiredPos, transform.rotation);

        levels.Enqueue(newLevel);
        // returns desiredPos to be used in providing the yOffset for the next level spawn
        return desiredPos;
    }

    // Dequeues and deletes the oldest level
    void deleteLevel()
    {
        GameObject levelToDelete = levels.Dequeue();
        Destroy(levelToDelete);
    }
}
