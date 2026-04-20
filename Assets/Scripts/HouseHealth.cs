using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HouseHealth : MonoBehaviour
{
    public static HouseHealth instance;

    [SerializeField] private int maxHealth = 5;
    [SerializeField] private TextMeshProUGUI healthText;

    private int currentHealth;

    private void Awake()
    {
        instance = this;
    }

    // Always start at 5 health.
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // Decrease health and end game if health reaches 0.
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateUI();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();
            GameOver();
        }
    }

    // Make sure health shows correct value.
    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    // Change to GameOver screen.
    private void GameOver()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveHighScore();
        }

        SceneManager.LoadScene("GameOver");
    }
}