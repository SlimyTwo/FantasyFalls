using UnityEngine;

public class EscapeOptionMenuScript : MonoBehaviour
{
    public GameObject escapeMenuCanvas; // Assign the Escape Menu Canvas in the Inspector
    public GameObject optionsMenuCanvas; // Assign the Options Menu Canvas in the Inspector

    public void BackToEscapeMenu()
    {
        // Show the escape menu and hide the options menu
        escapeMenuCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
    }

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
}