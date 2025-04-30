using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject mobileControlsPrefab;
    
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    
    private MobileControls mobileControls;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            SetupForPlatform();
        }
    }
    
    private void SetupForPlatform()
    {
        // If on mobile, instantiate mobile controls
        if (Application.isMobilePlatform)
        {
            if (mobileControlsPrefab != null)
            {
                GameObject controlsInstance = Instantiate(mobileControlsPrefab);
                mobileControls = controlsInstance.GetComponent<MobileControls>();
                DontDestroyOnLoad(controlsInstance);
            }
            else
            {
                Debug.LogError("Mobile controls prefab not assigned to GameManager!");
            }
            
            // Ensure screen doesn't go to sleep on mobile
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
    
    public bool IsMobilePlatform()
    {
        return Application.isMobilePlatform;
    }
    
    public void ToggleMobileControls(bool show)
    {
        if (mobileControls != null)
        {
            mobileControls.SetControlsActive(show);
        }
    }
}
