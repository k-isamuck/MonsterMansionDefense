using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuController : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restart clicked");
        SceneManager.LoadScene("GamePlay");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}