using System.Collections;
using UnityEngine;

public class frogMovement : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    public Sprite idleSprite;

    public Sprite leapSprite;

    // Called before start
    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement movement by solving equations later. Player should have to press enter to submit their solution.
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }

        // FROG IS NOT INTENDED TO MOVE BACKWARDS, THIS STATEMENT IS ONLY PRESENT FOR DEBUGGING AND SHOULD BE REMOVED BEFORE ADDING EQUATION MOVEMENT
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }


    }

    private void Move(Vector3 direction)
    {
        Vector3 movePos = transform.position + direction;

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
}
