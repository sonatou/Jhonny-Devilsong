using System;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyBase : MonoBehaviour
{

    [SerializeField] protected int maxHealth = 1;
    protected int _currentHealth;

    [SerializeField] protected int damage = 1;
    [SerializeField] protected float knockback = 3f;

    public event Action OnDeath;
    public event Action OnHit;

    protected Rigidbody2D _rb;

    protected Transform _target;
    protected Rigidbody2D _targetRb;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        _currentHealth = maxHealth;

        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _target = player.transform;
            _targetRb = player.GetComponent<Rigidbody2D>();
        }
    }

    protected virtual void Update()
    {
        FlipToPlayerPostion();
    }

    public virtual void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        OnHit?.Invoke();

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    protected virtual void FlipToPlayerPostion()
    {
        if(_target.transform.position.x - transform.position.x < 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }       
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);

        Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 direction = (col.transform.position - transform.position).normalized;
            playerRb.linearVelocity = direction * knockback;
        }
    }
}
