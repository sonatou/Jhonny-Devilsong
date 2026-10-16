using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int   maxHealth = 5;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private string gameOverScene = "GameOver";
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private float damageShake = 0.5f;
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private EventReference hurtSound;

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;

    private int _currentHealth;
    private bool _isInvincible = false;
    private bool _shielded;

    void Awake()
    {
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Hub" && !gameObject.activeSelf)
            Destroy(gameObject);
    }

    void Start()
    {
        PlayerHealth[] all = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        if (all.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        _currentHealth = maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible || _shielded || _currentHealth <= 0) return;

        ComboManager.Instance?.ResetCombo();
        CameraFeedback.Instance?.Shake(damageShake);
        if (hitVFX != null)
            Instantiate(hitVFX, transform.position, Quaternion.identity);
        if (!hurtSound.IsNull)
            RuntimeManager.PlayOneShot(hurtSound);

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            SceneTransition.Load(gameOverScene);
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(InvincibilityRoutine());
    }

    public void SetShielded(bool value)
    {
        _shielded = value;
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
