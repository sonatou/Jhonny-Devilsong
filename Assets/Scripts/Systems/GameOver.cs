using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text perfectText;

    void Start()
    {
        if (ScoreManager.Instance == null) return;

        scoreText.text   = $"Score: {ScoreManager.Instance.LastRunScore:N0}";
        perfectText.text = $"Perfects: {ScoreManager.Instance.LastRunPerfects}";
    }

    public void ReturnToHub()
    {
        ScoreManager.Instance.ResetRun();
        SceneManager.LoadScene("Hub");
    }
}
