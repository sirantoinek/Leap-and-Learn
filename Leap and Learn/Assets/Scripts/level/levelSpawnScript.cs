using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Diagnostics;

public class levelSpawnScript : MonoBehaviour
{
    public Transform frog;
    // Randomization can be added by creating different levels and selecting a random one to spawn (levels should be 10 units in height)
    public GameObject[] levelTemplates;
    public GameObject[] obstaclePrefabs;
    public GameObject[] platformPrefabs;
    public GameObject[] powerupPrefabs;
    public GameObject coinPrefab;
    public float[][] levelLanes;
    public float[][] waterLanes;
    public float[][] safeLanes;

    public float speedIncreaseRate;
    public int score;
    Vector3 lastPos; // Used to store previous spawn position
    Queue<GameObject> levels = new Queue<GameObject>(); // Queue to hold levels that are currently on screen
    Queue<GameObject> obstacles = new Queue<GameObject>(); // Queue to hold obstacles that are currently loaded
    public HashSet<GameObject> powerupSet = new HashSet<GameObject>(); // Hashset to hold powerups and coins that are currently loaded
    
    // Y positions where the frog drowns
    public Queue<Queue<int>> dangerLanes = new Queue<Queue<int>>();
    // Y positions where the frog respawns
    public Queue<Queue<int>> respawnLanes = new Queue<Queue<int>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int randomLevel = Random.Range(0, levelTemplates.Length);
        levelLanes = new float[4][];
        waterLanes = new float[4][];
        safeLanes = new float[4][];

        // Each point is worth 0.02f
        speedIncreaseRate = 0.02f;
        GameObject scoreValue = GameObject.Find("Score_Value");
        change_score scoreScript = scoreValue.GetComponent<change_score>();
        score = scoreScript.score;

        // Setting "lane" positions for levels
        levelLanes[0] = new float[] {2, 3, 4, 5};
        waterLanes[0] = new float[] {7, 8, 9};
        safeLanes[0] = new float[] {0, 1, 6};

        levelLanes[1] = new float[] {3, 4, 6, 7, 8};
        waterLanes[1] = new float[] {1};
        safeLanes[1] = new float[] {0, 2, 5, 9};

        levelLanes[2] = new float[] {1, 7, 8, 9};
        waterLanes[2] = new float[] {3, 4, 5};
        safeLanes[2] = new float[] {0, 2, 6};

        levelLanes[3] = new float[] {1, 2, 4, 5};
        waterLanes[3] = new float[] {7, 8};
        safeLanes[3] = new float[] {0, 3, 6, 9};

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
        int maxViewDistance = (int)lastPos.y - 3; // -2 by default as the player can see 2 tiles behind the frog (MADE to -3 to accomodate the new animation)
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
        dangerLanes.Dequeue();
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
        
        List<GameObject> coinsToRemove = new List<GameObject>();
        foreach (GameObject obj in powerupSet)
        {
            if (obj.transform.position.y <= (frog.position.y - 2))
            {
                coinsToRemove.Add(obj);
            }
        }
        foreach (GameObject obj in coinsToRemove)
        {
            powerupSet.Remove(obj);
            Destroy(obj);
        }
    }

    void spawnObstacles(float yOffset, GameObject level, int randomLevel)
    {
        // Set the safe positions
        Queue<int> grassLanes = new Queue<int>();
        for (int i = 0; i < safeLanes[randomLevel].Length; i++)
        {
            float respawnLaneY = safeLanes[randomLevel][i];
            int respawnY = (int)(yOffset + respawnLaneY);
            grassLanes.Enqueue(respawnY);
        }
        respawnLanes.Enqueue(grassLanes);

        // For each lane
        for (int i = 0; i < levelLanes[randomLevel].Length; i++)
        {
            float spawnLaneY = levelLanes[randomLevel][i];
            // You can mess around with the spawn range as much as you want. Numbers are just a place holder
            int numObstacles = Random.Range(1, 2); // Randomize how many obstacles spawn on the lane
            // Base speed 
            float baseSpeed = Random.Range(0.5f, 1.0f);
            float obstacleSpeed = Mathf.Min(baseSpeed + (score * speedIncreaseRate), 2.0f);

            for (int j = 0; j < numObstacles; j++)
            {
                // Randomize spawn position within the lane (X-axis)
                float spawnX = -5f;
                float xOffset = Random.Range(5f, 7f);
                Vector3 spawnPos = new Vector3(spawnX + (xOffset * j), yOffset + spawnLaneY, 0);

                // Instantiate the obstacle at the spawn position
                GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                obstacles.Enqueue(newObstacle);
                newObstacle.GetComponent<Obstacle>().SetMovementSpeed(obstacleSpeed);
            }
        }

        // Places where the frog drowns
        Queue<int> riverLanes = new Queue<int>();

        for (int i = 0; i < waterLanes[randomLevel].Length; i++)
        {
            // Set water lanes to check for drowning later
            float spawnLaneY = waterLanes[randomLevel][i];
            int waterLaneY = (int)(yOffset + spawnLaneY);
            riverLanes.Enqueue(waterLaneY);
 
            int logLength = Random.Range(0, platformPrefabs.Length);
            // The number of logs is different depending on length
            int numLogs;
            int logSize;
            float logOffset;
            GameObject platformPrefab = platformPrefabs[logLength];
            if (logLength == 0)
            {
                numLogs = 3;
                logSize = 7;
                logOffset = 11f;
            }
            else if (logLength == 1)
            {
                numLogs = 3;
                logSize = 5;
                logOffset = 7.5f;
            }
            else
            {
                numLogs = 4;
                logSize = 3;
                logOffset = 5f;
            }

            float platformSpeed = Random.Range(1f, 2f); // Place to adjust the speed later
            for (int j = 0; j < numLogs; j++) {
                float spawnX = -5f;
                Vector3 spawnPos = new Vector3(spawnX + (logOffset * j), yOffset + spawnLaneY, 0);

                GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
                newPlatform.GetComponent<Obstacle>().SetSize(logSize);
                obstacles.Enqueue(newPlatform);

                newPlatform.GetComponent<Obstacle>().SetMovementSpeed(platformSpeed);
            }
        }
        dangerLanes.Enqueue(riverLanes);

        int numCoins = Random.Range(2, 5);
        for (int i = 0; i < numCoins; i++)
        {
            float spawnX = Random.Range(-4, 4);
            float spawnY = Random.Range(-4, 4);

            Vector3 spawnPos = new Vector3(spawnX, spawnY + yOffset, 0);
            GameObject newCoin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            powerupSet.Add(newCoin);
        }

        int numPowerup = Random.Range(0, 2);
        for (int i = 0; i < numPowerup; i++)
        {
            float spawnX = Random.Range(-4, 4);
            float spawnY = Random.Range(-4, 4);

            Vector3 spawnPos = new Vector3(spawnX, spawnY + yOffset, 0);
            GameObject powerupPrefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
            GameObject newPowerup = Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
            powerupSet.Add(newPowerup);
        }
    }
}

