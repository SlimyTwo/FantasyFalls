using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float touchSensitivity = 50f; // Sensitivity for touch controls
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    
    // Sprint variables
    public float sprintSpeedMultiplier = 1.5f;
    private bool isSprinting = false;
    
    // Crouch variables
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeight = 0.6f;
    public float crouchTransitionSpeed = 10f;
    private float originalHeight;
    private Vector3 originalCameraPosition;
    private bool isCrouching = false;
    private float currentHeight;
    private float targetHeight;

    private CharacterController controller;
    private Transform playerCamera;
    private float verticalSpeed = 0f;
    private float xRotation = 0f;
    
    // Reference to mobile controls
    private MobileControls mobileControls;
    private bool usingTouchControls = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main.transform;
        originalHeight = controller.height;
        currentHeight = originalHeight;
        targetHeight = originalHeight;
        originalCameraPosition = playerCamera.localPosition;

        // Find mobile controls in scene if they exist
        mobileControls = FindObjectOfType<MobileControls>();
        
        // Set cursor state for PC
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Detect if we're on mobile platform
        usingTouchControls = Application.isMobilePlatform;
    }

    void Update()
    {
        HandleSprint();
        HandleCrouch();
        LookAround();
        MovePlayer();
    }

    void HandleSprint()
    {
        // Keyboard sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
        
        // Touch sprint
        if (mobileControls != null && usingTouchControls)
        {
            isSprinting = mobileControls.IsSprinting();
        }
    }

    void HandleCrouch()
    {
        // Check for crouch toggle from keyboard or mobile
        bool shouldToggleCrouch = Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl);
        
        // Also check mobile controls
        if (mobileControls != null && usingTouchControls)
        {
            if (mobileControls.IsCrouching() != isCrouching)
            {
                shouldToggleCrouch = true;
            }
        }
        
        if (shouldToggleCrouch)
        {
            isCrouching = !isCrouching;
            targetHeight = isCrouching ? crouchHeight : originalHeight;
        }
        
        // Smoothly transition height
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        
        // Snap to target height if very close to complete the transition
        if (Mathf.Abs(currentHeight - targetHeight) < 0.01f)
        {
            currentHeight = targetHeight;
        }
        
        // Update controller height and center
        controller.height = currentHeight;
        controller.center = new Vector3(0, currentHeight / 2, 0);
        
        // Check for obstacles when standing up
        if (!isCrouching && currentHeight < originalHeight)
        {
            if (Physics.Raycast(transform.position, Vector3.up, originalHeight))
            {
                // Don't stand up if there's an obstacle above
                currentHeight = controller.height;
                targetHeight = currentHeight;
                isCrouching = true;
            }
        }
        
        // Calculate camera offset based on height difference
        float heightDifference = originalHeight - currentHeight;
        float cameraOffset = heightDifference / 2;
        
        // Update camera position
        playerCamera.localPosition = new Vector3(
            originalCameraPosition.x, 
            originalCameraPosition.y - cameraOffset, 
            originalCameraPosition.z
        );
    }

    void LookAround()
    {
        float mouseX = 0f;
        float mouseY = 0f;
        
        // Get input based on platform
        if (usingTouchControls && mobileControls != null)
        {
            // Only use right joystick for camera when it's not being used for movement
            Vector2 lookInput = mobileControls.GetLookInput();
            mouseX = lookInput.x * touchSensitivity * Time.deltaTime;
            mouseY = lookInput.y * touchSensitivity * Time.deltaTime;
        }
        else
        {
            // Use mouse for desktop
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        float x = 0f;
        float z = 0f;
        
        // Get input based on platform
        if (usingTouchControls && mobileControls != null)
        {
            // Use the left joystick for movement on mobile
            Vector2 movementInput = mobileControls.GetMovementInput();
            x = movementInput.x;
            z = movementInput.y;
        }
        else
        {
            // Use keyboard for desktop
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded && verticalSpeed < 0)
            verticalSpeed = -2f;

        // Handle jumping from keyboard or mobile
        bool shouldJump = Input.GetButtonDown("Jump");
        if (mobileControls != null && usingTouchControls)
        {
            shouldJump = shouldJump || mobileControls.GetJumpInput();
        }
        
        if (shouldJump && controller.isGrounded)
            verticalSpeed = jumpForce;

        verticalSpeed += gravity * Time.deltaTime;
        
        // Apply vertical movement separately
        Vector3 verticalMove = new Vector3(0, verticalSpeed * Time.deltaTime, 0);
        
        // Calculate final movement speed based on sprint and crouch states
        float currentSpeed = speed;
        if (isSprinting && !isCrouching)
        {
            currentSpeed *= sprintSpeedMultiplier;
        }
        else if (isCrouching)
        {
            currentSpeed *= crouchSpeedMultiplier;
        }
        
        // Move horizontally with modified speed
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Move vertically
        controller.Move(verticalMove);
    }
}
