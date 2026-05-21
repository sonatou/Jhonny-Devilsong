using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int   maxHealth            = 5;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float flashInterval         = 0.1f;
    [SerializeField] private string gameOverScene        = "GameOver";

    public event Action<int> OnHealthChanged;
    public event Action      OnDeath;

    public int CurrentHealth => _currentHealth;

    private int            _currentHealth;
    private bool           _isInvincible = false;
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            ScoreManager.Instance?.SaveRun();
            Destroy(gameObject);
            SceneManager.LoadScene(gameOverScene);
            return;
        }

        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        _isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            _sr.enabled = !_sr.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        _sr.enabled   = true;
        _isInvincible = false;
    }
}
