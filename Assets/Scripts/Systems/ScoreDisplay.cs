using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        BindScore();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindScore();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnbindScore();
        BindScore();
    }

    void BindScore()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.OnScoreChanged += UpdateScore;
        ScoreManager.Instance.OnComboChanged += UpdateCombo;

        UpdateScore(ScoreManager.Instance.CurrentScore);
        UpdateCombo(ScoreManager.Instance.CurrentCombo);
    }

    void UnbindScore()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        ScoreManager.Instance.OnComboChanged -= UpdateCombo;
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString("N0");
    }

    void UpdateCombo(int combo)
    {
        if (comboText == null) return;

        comboText.gameObject.SetActive(combo > 1);
        comboText.text = $"x{combo}";
    }
}
