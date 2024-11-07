using UnityEngine;
using System.Collections.Generic;

public class levelSpawnScript : MonoBehaviour
{

    public Transform frog;
    // Randomization can be added by creating different levels and selecting a random one to spawn (levels should be 10 units in height)
    public GameObject[] levelTemplates;
    public GameObject[] obstaclePrefabs;
    public float[][] levelLanes;


    Vector3 lastPos; // Used to store previous spawn position
    Queue<GameObject> levels = new Queue<GameObject>(); // Queue to hold levels that are currently on screen
    Queue<GameObject> obstacles = new Queue<GameObject>(); // Queue to hold obstacles that are currently loaded

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int randomLevel = Random.Range(0, levelTemplates.Length);
        levelLanes = new float[4][];

        // Setting "lane" positions for levels
        levelLanes[0] = new float[] {2, 3, 4, 5};
        levelLanes[1] = new float[] {3, 4, 6, 7, 8};
        levelLanes[2] = new float[] {1, 7, 8, 9};
        levelLanes[3] = new float[] {1, 2, 4, 5};

        // Initializing the starting area
        lastPos = spawnLevel(-25, randomLevel);
        lastPos = spawnLevel((int)lastPos.y, randomLevel);
        lastPos = spawnLevel((int)lastPos.y, randomLevel);
    }

    // Update is called once per frame
    void Update()
    {
        int randomLevel = Random.Range(0, levelTemplates.Length);

        // maxViewDistance is the furthest point a player can be before they can see off the edge of the "level" with the camera
        int maxViewDistance = (int)lastPos.y - 2; // -2 by default as the player can see 2 tiles behind the frog
        if (frog.position.y >= maxViewDistance)
        {
            // Spawn a new level and delete the oldest level that is now offscreen
            lastPos = spawnLevel((int)lastPos.y, randomLevel);
            deleteLevel();
            deleteObstacles();
        }
    }

    Vector3 spawnLevel(int yOffset, int randomLevel)
    {
        // Offset the stationary levelSpawner position with yOffset to place new levels in the correct location
        Vector3 desiredPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        GameObject newLevel = Instantiate(levelTemplates[randomLevel], desiredPos, transform.rotation);

        levels.Enqueue(newLevel);
        spawnObstacles(desiredPos.y, newLevel, randomLevel);
        // returns desiredPos to be used in providing the yOffset for the next level spawn
        return desiredPos;
    }

    // Dequeues and deletes the oldest level and obstacles
    void deleteLevel()
    {
        GameObject levelToDelete = levels.Dequeue();
        Destroy(levelToDelete);
    }

    // Dequeues and deletes obstacles that are out of the camera view
    void deleteObstacles()
    {
        // If the oldest obstacle in the queue has a y position that is outside the camera view (frog.position.y - 2), dequeue and delete
        while (obstacles.Count > 0 && obstacles.Peek().transform.position.y <= (frog.position.y - 2))
        {
            GameObject obstacleToDelete = obstacles.Dequeue();
            Destroy(obstacleToDelete);
        }
    }

    void spawnObstacles(float yOffset, GameObject level, int randomLevel)
    {
        // You can mess around with the spawn range as much as you want. Numbers just a place holder
        int numObstacles = Random.Range(7, 10); // Randomize how many obstacles spawn on the level

        for (int i = 0; i < numObstacles; i++)
        {
            // Can probably set the if statement here to choose the right level lanes.
            // Randomly choose a lane from the obstacleLanes array
            float spawnLaneY = levelLanes[randomLevel][Random.Range(0, levelLanes[randomLevel].Length)];

            // Randomize spawn position within the lane (X-axis)
            float spawnX = Random.Range(-5f, 5f);
            Vector3 spawnPos = new Vector3(spawnX, yOffset + spawnLaneY, 0);

            // Instantiate the obstacle at the spawn position
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            obstacles.Enqueue(newObstacle);

            float obstacleSpeed = 1f; // Place to adjust the speed later
            newObstacle.GetComponent<Obstacle>().SetMovementSpeed(obstacleSpeed);
        }
    }
}