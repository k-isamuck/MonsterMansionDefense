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

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer = 0f;
            AddScore(10);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public int GetScore()
    {
        return score;
    }

    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}