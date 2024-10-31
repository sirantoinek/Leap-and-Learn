using UnityEngine;

public class frogMovement : MonoBehaviour
{
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
            Move(Vector3.left);
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector3.up);
        }

        // FROG IS NOT INTENDED TO MOVE BACKWARDS, THIS STATEMENT IS ONLY PRESENT FOR DEBUGGING AND SHOULD BE REMOVED BEFORE ADDING EQUATION MOVEMENT
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector3.down);
        }


    }

    private void Move(Vector3 direction)
    {
        Vector3 movePos = transform.position + direction;

        // Prevents the player from moving off the edge of the screen
        if ((movePos.x >= -5) && (movePos.x <= 5))
        {
            transform.position = movePos;
        }
    }
}
