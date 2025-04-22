using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 50f; // Movement speed

    void Update()
    {
        // Get horizontal and vertical input (A/D or Left/Right arrow keys, W/S or Up/Down arrow keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement
        Vector3 movement = new Vector3(horizontalInput * speed * Time.deltaTime, verticalInput * speed * Time.deltaTime, 0);

        // Apply movement to the player's position
        transform.position += movement;
    }
}

// First Git Commit Test