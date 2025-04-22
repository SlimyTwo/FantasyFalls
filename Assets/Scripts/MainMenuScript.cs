using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel; // Assign the MainMenuPanel in the Inspector
    public GameObject optionsPanel; // Assign the OptionsPanel in the Inspector

    public void StartGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene");
    }

    public void OpenOptions()
    {
        // Show the options panel and hide the main menu panel
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game");
        Application.Quit();
    }
}