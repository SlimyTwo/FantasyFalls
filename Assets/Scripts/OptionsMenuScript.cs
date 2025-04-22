using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenuPanel; // Assign the MainMenuPanel in the Inspector
    public GameObject optionsPanel; // Assign the OptionsPanel in the Inspector

    public void ToggleFullscreen()
    {
        if (!Screen.fullScreen)
        {
            // Switching to fullscreen - force 1920x1080
            Screen.SetResolution(1920, 1080, true);
            Debug.Log("Fullscreen enabled at 1920x1080");
        }
        else
        {
            // Switching to windowed
            Screen.fullScreen = false;
            Debug.Log("Windowed mode enabled");
        }
    }

    public void BackToMainMenu()
    {
        // Show the main menu panel and hide the options panel
        if (mainMenuPanel != null && optionsPanel != null)
        {
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("MainMenuPanel or OptionsPanel is not assigned in the Inspector.");
        }
    }
}