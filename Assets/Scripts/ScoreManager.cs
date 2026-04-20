using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;
    private float timer = 0f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    // Increase score by 10 for every second.
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer = 0f;
            AddScore(10);
        }
    }

    // Update score counter.
    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    // Return the score.
    public int GetScore()
    {
        return score;
    }

    // Log and update the highscore.
    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    // Make sure visible score counter is upodated to correct score.
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}