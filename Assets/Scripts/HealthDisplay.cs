using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image[] hearts;
    public Color aliveColor = Color.red;
    public Color deadColor = new Color(1f, 1f, 1f, 0.2f);

    private PlayerHealth _playerHealth;

    void Start()
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (_playerHealth == null) return;

        _playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts(_playerHealth.CurrentHealth);
    }

    void OnDisable()
    {
        if (_playerHealth != null)
            _playerHealth.OnHealthChanged -= UpdateHearts;
    }

    void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].color = i < currentHealth ? aliveColor : deadColor;
    }
}