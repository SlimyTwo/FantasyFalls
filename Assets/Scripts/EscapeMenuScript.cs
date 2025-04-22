using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuScript : MonoBehaviour
{
    public GameObject escapeMenuCanvas; // Assign the Canvas in the Inspector
    public GameObject optionsMenuCanvas; // Assign the Options Menu Canvas in the Inspector

    private bool isMenuActive = false;
    
    void Start()
    {
        // Ensure escape menu is hidden when the game starts
        isMenuActive = false;
        escapeMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(false);
    
        // Make sure game is unpaused
        Time.timeScale = 1f;
    
        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If options menu is active, go back to escape menu
            if (optionsMenuCanvas.activeSelf)
            {
                BackToEscapeMenu();
            }
            // Otherwise toggle the escape menu
            else
            {
                ToggleEscapeMenu();
            }
        }
    }

    public void ToggleEscapeMenu()
    {
        isMenuActive = !isMenuActive;
        escapeMenuCanvas.SetActive(isMenuActive);

        // Pause or resume the game
        Time.timeScale = isMenuActive ? 0f : 1f;

        // Show or hide the cursor
        Cursor.lockState = isMenuActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenuActive;
    }
    public void ResumeGame()
    {
        isMenuActive = false;
        escapeMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void OpenOptions()
    {
        // Show the options menu and hide the escape menu
        escapeMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
    }
    
    public void BackToEscapeMenu()
    {
        // Show the escape menu and hide the options menu
        escapeMenuCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
    }
    
    public void QuitToMainMenu()
    {
        // Reset time scale in case the game is paused
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}