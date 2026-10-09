using System;
using System.Collections;
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
    [SerializeField] protected float flashDuration = 0.1f;
    [SerializeField] private ParticleSystem deathVFX;
    private Color _originalColor;

    [Header("Musica")]
    [SerializeField] private AnimationCurve beatStepCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private float minBeatSpeed = 0.3f;
    [SerializeField] private float bassSpeedBoost = 0.5f;
    [SerializeField] private float bassScalePulse = 0.25f;

    [Header("Squash no dano")]
    [SerializeField] private Vector3 hitSquash = new Vector3(1.25f, 0.75f, 1f);
    [SerializeField] private float squashDuration = 0.12f;

    protected float MusicSpeedMultiplier { get; private set; } = 1f;

    private Vector3 _baseScale;
    private float _squashTimer;

protected virtual void Start()
    {
        _currentHealth = maxHealth;
        _originalColor = spriteRenderer.color;
        _baseScale = spriteRenderer.transform.localScale;

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
        ApplyMusic();
    }

    void ApplyMusic()
    {
        float bass = AudioSpectrum.Instance != null ? AudioSpectrum.Instance.Bass : 0f;

        float step = 1f;
        if (BeatManager.Instance != null)
        {
            float phase = (float)((BeatManager.Clock - BeatManager.Instance.LastBeatTime) / BeatManager.Instance.BeatInterval);
            step = beatStepCurve.Evaluate(Mathf.Clamp01(phase));
        }

        MusicSpeedMultiplier = Mathf.Lerp(minBeatSpeed, 1f + bass * bassSpeedBoost, step);

        float squash = squashDuration > 0f ? _squashTimer / squashDuration : 0f;
        _squashTimer = Mathf.Max(0f, _squashTimer - Time.deltaTime);
        Vector3 squashScale = Vector3.Lerp(Vector3.one, hitSquash, squash);
        spriteRenderer.transform.localScale = Vector3.Scale(_baseScale * (1f + bass * bassScalePulse), squashScale);
    }

    public virtual void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        OnHit?.Invoke();
        _squashTimer = squashDuration;
        StartCoroutine(FlashRoutine());

        if (_currentHealth <= 0)
        {
            if (deathVFX != null)
                Instantiate(deathVFX, transform.position, Quaternion.identity);
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

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = _originalColor;
    }
}
