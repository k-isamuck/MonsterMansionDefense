using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuController : MonoBehaviour
{
    // Start game again.
    public void RestartGame()
    {
        Debug.Log("Restart clicked");
        SceneManager.LoadScene("GamePlay");
    }

    // Button to go back to main menu.
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}