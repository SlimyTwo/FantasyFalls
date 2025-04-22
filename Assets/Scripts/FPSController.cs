using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    
    // Sprint variables
    public float sprintSpeedMultiplier = 1.5f;
    private bool isSprinting = false;
    
    // Crouch variables
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeight = 0.6f;  // Reduced crouch height for lower crouch
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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main.transform;
        originalHeight = controller.height;
        currentHeight = originalHeight;
        targetHeight = originalHeight;
        originalCameraPosition = playerCamera.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        // Toggle sprint when Left Shift is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }

    void HandleCrouch()
    {
        // // Toggle crouch when C or Left Control is pressed
        // if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     isCrouching = !isCrouching;
        //     targetHeight = isCrouching ? crouchHeight : originalHeight;
        // }
        //
        // // Smoothly transition height
        // currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        //
        // // Snap to target height if very close to complete the transition
        // if (Mathf.Abs(currentHeight - targetHeight) < 0.01f)
        // {
        //     currentHeight = targetHeight;
        // }
        //
        // // Update controller height and center
        // controller.height = currentHeight;
        // controller.center = new Vector3(0, currentHeight / 2, 0);
        //
        // // Check for obstacles when standing up
        // if (!isCrouching && currentHeight < originalHeight)
        // {
        //     if (Physics.Raycast(transform.position, Vector3.up, originalHeight))
        //     {
        //         // Don't stand up if there's an obstacle above
        //         currentHeight = controller.height;
        //         targetHeight = currentHeight;
        //         isCrouching = true;
        //     }
        // }
        //
        // // Calculate camera offset based on height difference
        // float heightDifference = originalHeight - currentHeight;
        // float cameraOffset = heightDifference / 2;
        //
        // // Update camera position
        // playerCamera.localPosition = new Vector3(
        //     originalCameraPosition.x, 
        //     originalCameraPosition.y - cameraOffset, 
        //     originalCameraPosition.z
        // );
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Debug the input values
        Debug.Log($"Input values: Horizontal = {x}, Vertical = {z}");

        Vector3 move = transform.right * x + transform.forward * z;
        
        // Debug the move vector before applying vertical speed
        Debug.Log($"Move vector (before vertical): {move}");

        if (controller.isGrounded && verticalSpeed < 0)
            verticalSpeed = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
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
        
        // Debug the controller's state
        Debug.Log($"isGrounded: {controller.isGrounded}, verticalSpeed: {verticalSpeed}, isSprinting: {isSprinting}, isCrouching: {isCrouching}");
    }
}
