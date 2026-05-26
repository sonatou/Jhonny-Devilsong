using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
            scoreText.text = $"Score: {ScoreManager.Instance.LastRunScore:N0}";
    }

    public void ReturnToHub()
    {
        ScoreManager.Instance?.ResetRun();
        GameManager.Instance?.ResetRooms();
        SceneManager.LoadScene("Hub");
    }

    public void NewRun()
    {
        ScoreManager.Instance?.ResetRun();
        GameManager.Instance?.ResetRooms();
        SceneManager.LoadScene("Main");
    }
}