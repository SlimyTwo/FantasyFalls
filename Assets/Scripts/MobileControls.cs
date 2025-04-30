using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    [Header("Joystick References")]
    public RectTransform leftJoystickBackground;
    public RectTransform leftJoystickHandle;
    public RectTransform rightJoystickBackground;
    public RectTransform rightJoystickHandle;
    
    [Header("Action Buttons")]
    public Button jumpButton;
    public Button crouchButton;
    public Button sprintButton;
    
    [Header("Joystick Settings")]
    public float joystickRadius = 50f;
    public float rightJoystickSpeedMultiplier = 2.0f; // Right joystick moves faster
    
    // Joystick values
    private Vector2 leftJoystickInput;
    private Vector2 rightJoystickInput;
    
    // Touch tracking
    private int leftJoystickTouchID = -1;
    private int rightJoystickTouchID = -1;
    
    // Action button states
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isSprinting = false;
    
    private Canvas parentCanvas;
    private bool controlsActive = false;
    
    // Tracking which joystick is currently active for movement
    private bool isRightJoystickActive = false;
    private bool isLeftJoystickActive = false;
    
    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        
        // Initialize buttons if they exist
        if (jumpButton != null)
            jumpButton.onClick.AddListener(() => isJumping = true);
        
        if (crouchButton != null)
            crouchButton.onClick.AddListener(ToggleCrouch);
        
        if (sprintButton != null)
        {
            sprintButton.GetComponent<Button>().onClick.AddListener(ToggleSprint);
        }
        
        // Only show controls on mobile platforms
        controlsActive = Application.isMobilePlatform;
        SetControlsActive(controlsActive);
    }

    public void SetControlsActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
        controlsActive = active;
    }
    
    void Update()
    {
        if (!controlsActive)
            return;
            
        ProcessTouchInput();
        
        // Reset jump after one frame
        if (isJumping)
            isJumping = false;
    }
    
    void ProcessTouchInput()
    {
        // Update active joystick states
        isLeftJoystickActive = leftJoystickTouchID != -1;
        isRightJoystickActive = rightJoystickTouchID != -1;
        
        // Process all touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            Vector2 touchPosition = GetTouchPositionOnCanvas(touch.position);
            
            // Left half of screen = movement joystick
            if (touch.position.x < Screen.width / 2)
            {
                // Skip if right joystick is active and we're enforcing one at a time
                if (isRightJoystickActive && touch.phase == TouchPhase.Began)
                    continue;
                    
                HandleLeftJoystick(touch, touchPosition);
            }
            // Right half of screen = camera/fast movement joystick
            else
            {
                // Skip if left joystick is active and we're enforcing one at a time
                if (isLeftJoystickActive && touch.phase == TouchPhase.Began)
                    continue;
                    
                HandleRightJoystick(touch, touchPosition);
            }
        }
        
        // Reset joysticks if needed
        if (leftJoystickTouchID != -1 && !IsTouchActive(leftJoystickTouchID))
        {
            ResetLeftJoystick();
        }
        
        if (rightJoystickTouchID != -1 && !IsTouchActive(rightJoystickTouchID))
        {
            ResetRightJoystick();
        }
    }
    
    private void HandleLeftJoystick(Touch touch, Vector2 touchPosition)
    {
        if (leftJoystickBackground == null) return;
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (leftJoystickTouchID == -1)
                {
                    leftJoystickTouchID = touch.fingerId;
                    leftJoystickBackground.position = touchPosition;
                }
                break;
                
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (touch.fingerId == leftJoystickTouchID)
                {
                    Vector2 offset = touchPosition - (Vector2)leftJoystickBackground.position;
                    Vector2 direction = offset.magnitude > joystickRadius ? 
                                        offset.normalized : offset / joystickRadius;
                    leftJoystickInput = direction;
                    leftJoystickHandle.position = leftJoystickBackground.position + 
                                                (Vector3)(direction * joystickRadius);
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (touch.fingerId == leftJoystickTouchID)
                {
                    ResetLeftJoystick();
                }
                break;
        }
    }
    
    private void HandleRightJoystick(Touch touch, Vector2 touchPosition)
    {
        if (rightJoystickBackground == null) return;
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (rightJoystickTouchID == -1)
                {
                    rightJoystickTouchID = touch.fingerId;
                    rightJoystickBackground.position = touchPosition;
                }
                break;
                
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (touch.fingerId == rightJoystickTouchID)
                {
                    Vector2 offset = touchPosition - (Vector2)rightJoystickBackground.position;
                    Vector2 direction = offset.magnitude > joystickRadius ? 
                                        offset.normalized : offset / joystickRadius;
                    rightJoystickInput = direction;
                    rightJoystickHandle.position = rightJoystickBackground.position + 
                                                 (Vector3)(direction * joystickRadius);
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (touch.fingerId == rightJoystickTouchID)
                {
                    ResetRightJoystick();
                }
                break;
        }
    }
    
    private void ResetLeftJoystick()
    {
        leftJoystickTouchID = -1;
        leftJoystickInput = Vector2.zero;
        isLeftJoystickActive = false;
        if (leftJoystickHandle != null && leftJoystickBackground != null)
            leftJoystickHandle.position = leftJoystickBackground.position;
    }
    
    private void ResetRightJoystick()
    {
        rightJoystickTouchID = -1;
        rightJoystickInput = Vector2.zero;
        isRightJoystickActive = false;
        if (rightJoystickHandle != null && rightJoystickBackground != null)
            rightJoystickHandle.position = rightJoystickBackground.position;
    }
    
    private bool IsTouchActive(int touchId)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == touchId)
                return true;
        }
        return false;
    }
    
    private Vector2 GetTouchPositionOnCanvas(Vector2 screenPosition)
    {
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return screenPosition;
            
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(), 
            screenPosition, 
            parentCanvas.worldCamera, 
            out localPoint);
            
        return parentCanvas.transform.TransformPoint(localPoint);
    }
    
    public Vector2 GetMovementInput()
    {
        // If right joystick is active, use it for movement (with speed multiplier)
        if (isRightJoystickActive)
        {
            return rightJoystickInput * rightJoystickSpeedMultiplier;
        }
        // Otherwise use left joystick for movement
        return leftJoystickInput;
    }
    
    public Vector2 GetLookInput()
    {
        // Only use right joystick for looking when it's not being used for movement
        if (!isRightJoystickActive)
        {
            return rightJoystickInput;
        }
        return Vector2.zero;
    }
    
    public bool GetJumpInput()
    {
        return isJumping;
    }
    
    public bool IsCrouching()
    {
        return isCrouching;
    }
    
    public bool IsSprinting()
    {
        return isSprinting;
    }
    
    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        if (crouchButton != null)
        {
            Color buttonColor = crouchButton.GetComponent<Image>().color;
            buttonColor.a = isCrouching ? 0.8f : 0.5f;
            crouchButton.GetComponent<Image>().color = buttonColor;
        }
    }
    
    public void ToggleSprint()
    {
        isSprinting = !isSprinting;
        if (sprintButton != null)
        {
            Color buttonColor = sprintButton.GetComponent<Image>().color;
            buttonColor.a = isSprinting ? 0.8f : 0.5f;
            sprintButton.GetComponent<Image>().color = buttonColor;
        }
    }
}
