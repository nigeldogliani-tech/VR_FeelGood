using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    [SerializeField] private int score = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        // Singleton : accès au ScoreManager depuis les autres scripts.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        UpdateScoreUI();
    }

    public void AddPoint()
    {
        score++;

        UpdateScoreUI();

        Debug.Log("Score actuel : " + score);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + score;
        }
    }
}
