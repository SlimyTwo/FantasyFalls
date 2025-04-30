using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 50f; // Movement speed
    
    private MobileControls mobileControls;
    
    void Start()
    {
        // Find mobile controls in scene if they exist
        mobileControls = FindObjectOfType<MobileControls>();
    }

    void Update()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        // Check for keyboard input first
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        // If there's no keyboard input and mobile controls exist, use mobile input
        if (Mathf.Approximately(horizontalInput, 0) && Mathf.Approximately(verticalInput, 0) && mobileControls != null)
        {
            Vector2 mobileInput = mobileControls.GetMovementInput();
            horizontalInput = mobileInput.x;
            verticalInput = mobileInput.y;
        }

        // Calculate movement
        Vector3 movement = new Vector3(horizontalInput * speed * Time.deltaTime, verticalInput * speed * Time.deltaTime, 0);

        // Apply movement to the player's position
        transform.position += movement;
    }
}

// Test Git Commit Test
