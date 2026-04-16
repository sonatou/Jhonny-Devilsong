using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    private int _currentHealth;

    public int CurrentHealth => _currentHealth;

    void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}