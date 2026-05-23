using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Image[] hearts;
    [SerializeField] private Color aliveColor = Color.red;
    [SerializeField] private Color deadColor  = new Color(1f, 1f, 1f, 0.2f);

    private PlayerHealth _playerHealth;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindAndBindPlayer();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindPlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnbindPlayer();
        FindAndBindPlayer();
    }

void FindAndBindPlayer()
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (_playerHealth == null) return;

        _playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts(_playerHealth.MaxHealth);
    }

    void UnbindPlayer()
    {
        if (_playerHealth == null) return;
        _playerHealth.OnHealthChanged -= UpdateHearts;
        _playerHealth = null;
    }

    void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].color = i < currentHealth ? aliveColor : deadColor;
    }
}
