using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;

    public event Action OnDeath;
    public event Action OnHit;


    private int _currentHealth;

    void Start()
    {
        _currentHealth = maxHealth;
    }

public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        OnHit?.Invoke();

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}