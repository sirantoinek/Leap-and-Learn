using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class frogMovement : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public QuestionManager questionManager;
    public AnswerField answerField;
    // varibale for the user input box
    public TMP_InputField answerInputField;
    public TextMeshProUGUI textDisplay;
    
    // if the frog can move or not
    public bool canMove;
    public bool hasMoved;
    public string currentQuestion;

    private frogHealth health;

    public Sprite idleSprite;

    public Sprite leapSprite;

    // Called before start
    public void Awake()
    {
        health = GetComponent<frogHealth>();

        canMove = false;
        hasMoved = true;
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
        if (!hasMoved) {
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
        if (obstacle != null)
        {
            health.LoseHeart();
        }

        
        // Prevents the player from moving off the edge of the screen
        if ((movePos.x >= -5) && (movePos.x <= 5))
        {
            StartCoroutine(LeapAnimation(movePos));
        }
    }

    private IEnumerator LeapAnimation(Vector3 destination)
    {
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
    }

    public void unlockMovement() {
        canMove = true;
    }

    public void lockMovement() {
        canMove = false;
    }
}
