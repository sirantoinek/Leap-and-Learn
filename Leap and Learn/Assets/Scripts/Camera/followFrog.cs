using UnityEngine;

public class followPlayer : MonoBehaviour
{
    public Transform frog;
    public int yOffset; // Alters the position of the player on the screen. Default value should be 5. If this number is to be changed, levelSpawn must be adjusted accordingly

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        updateCamera(); // Using LateUpdate to prevent camera jitter
    }

    // Keeps the camera on the frog
    void updateCamera()
    {
        Vector3 desiredPos = new Vector3(transform.position.x, frog.position.y + yOffset, transform.position.z); ;
        transform.position = desiredPos;
    }
}
