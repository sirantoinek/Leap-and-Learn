using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class frogMovement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public QuestionManager questionManager;
    public AnswerField answerField;
    // Variable for the user input box
    public TMP_InputField answerInputField;
    public TextMeshProUGUI textDisplay;

    // Locations for the frog to interact with
    Queue<Queue<int>> dangerLanes;
    Queue<Queue<int>> respawnLanes;

    // If the frog can move or not
    public bool canMove;
    public bool hasMoved;
    public string currentQuestion;

    private frogHealth health;

    public Sprite idleSprite;
    public Sprite leapSprite;

    public int respawnLocation;


    // Called before start
    public void Awake()
    {
        health = GetComponent<frogHealth>();
        respawnLocation = -5;
        canMove = true;
        hasMoved = true;

        // Get the lane info
        GameObject levelSpawner = GameObject.Find("levelSpawner");
        levelSpawnScript levelScript = levelSpawner.GetComponent<levelSpawnScript>();
        dangerLanes = levelScript.dangerLanes;
        respawnLanes = levelScript.respawnLanes;

        spriteRenderer = GetComponent<SpriteRenderer>();
        textDisplay = GameObject.Find("AnswerTextUI").GetComponent<TextMeshProUGUI>();
        // finds the specific UI TextInput which allows us to actually use the input box in relation to movement
        answerInputField = GameObject.Find("AnswerInputField").GetComponent<TMP_InputField>();
        // for debug purposes
        if (answerInputField == null) {
            Debug.LogError("AnswerInputField not found in scene...");
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        answerInputField.ActivateInputField();
        answerField.displayNewQuestion();
    }
    

    // Update is called once per frame
    void Update()
    {
        // hasMoved set to allow movement only after correct answer in AnswerField.cs
        if (!hasMoved && canMove) {
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                Move(Vector3.left);
                //makes sure player can't enter this loop to move
                hasMoved = true;
                // puts player curser right back into selcting the answerfield so that they can only move one time
                // also resets the field so there isn't answer from last time still sitting in it
                answerInputField.text = "";
                answerInputField.ActivateInputField();
            }
            else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                Move(Vector3.right);
                hasMoved = true;
                answerInputField.text = "";
                answerInputField.ActivateInputField();
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Move(Vector3.up);
                hasMoved = true;
                answerInputField.text = "";
                answerInputField.ActivateInputField();
            }

            // FROG IS NOT INTENDED TO MOVE BACKWARDS, THIS STATEMENT IS ONLY PRESENT FOR DEBUGGING AND SHOULD BE REMOVED BEFORE ADDING EQUATION MOVEMENT
            /*
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                Move(Vector3.down);
            }
            */
        }

    }

    private void Move(Vector3 direction)
    {
        Vector3 movePos = transform.position + direction;

        if ((movePos.x >= -5) && (movePos.x <= 5))
        {
            StartCoroutine(LeapAnimation(movePos));
        }

        Collider2D platform = Physics2D.OverlapBox(movePos, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(movePos, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));

        if (platform != null)
        {
            transform.SetParent(platform.transform);
        }
        else
        {
            transform.SetParent(null);
        }

        // Check if drowning
        if (transform.parent == null && checkIfLane((int)movePos.y, dangerLanes))
        {
            health.LoseHeart();
            respawn();
        }
        // Check if safe
        if (checkIfLane((int)movePos.y, respawnLanes))
        {
            respawnLocation = (int)movePos.y;
            Debug.Log("New Respawn Point");
            Debug.Log(respawnLocation);
        }

    }

    private IEnumerator LeapAnimation(Vector3 destination)
    {
        lockMovement();
        Vector3 startPos = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

        spriteRenderer.sprite = leapSprite;

        while (elapsed < duration)
        {
            float time = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, destination, time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;

        spriteRenderer.sprite = idleSprite;
        unlockMovement();
    }

    public void unlockMovement() {
        canMove = true;
    }

    public void lockMovement() {
        canMove = false;
    }

    private bool checkIfLane(int location, Queue<Queue<int>> lanes)
    {
        foreach (Queue<int> innerQueue in lanes)
        {
            foreach (int lane in innerQueue)
            {
                if (lane == location)
                {
                    return true; 
                }
            }
        }
        return false;
    }

    private void respawn()
    {
        StopAllCoroutines();
        Debug.Log("Respawn");
        Vector3 spawnPoint = new Vector3(0, respawnLocation, 0);
        transform.position = spawnPoint;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            health.LoseHeart();
            respawn();
        }
    }
}
