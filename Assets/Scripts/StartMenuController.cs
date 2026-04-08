using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // shows in editor
    }
}