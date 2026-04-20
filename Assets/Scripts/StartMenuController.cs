using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    // Play GamePlay scene on click.
    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    // Quit game when other button is clicked.
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // shows in editor
    }
}