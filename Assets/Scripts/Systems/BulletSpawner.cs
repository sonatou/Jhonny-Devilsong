using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BulletSpawner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private BeatIndicator beatIndicator;

    [Header("Projétil")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Cooldown")]
    [SerializeField] private float fireCooldown = 0.3f;

    [Header("Perfect")]
    [SerializeField] private float perfectSpeed = 14f;
    [SerializeField] private int perfectDamage = 3;
    [SerializeField] private float perfectScale = 1.5f;
    [SerializeField] private Color perfectColor = Color.yellow;

    [Header("Good")]
    [SerializeField] private float goodSpeed = 10f;
    [SerializeField] private int goodDamage = 2;
    [SerializeField] private float goodScale = 1f;
    [SerializeField] private Color goodColor = Color.white;

    [Header("Miss")]
    [SerializeField] private float missSpeed = 7f;
    [SerializeField] private int missDamage = 1;
    [SerializeField] private float missScale = 0.6f;
    [SerializeField] private Color missColor = new Color(1f, 1f, 1f, 0.4f);

    private InputSystem_Actions _actions;
    private float _lastFireTime = -Mathf.Infinity;

    void Awake()
    {
        _actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        _actions.Player.Attack.performed -= OnAttack;
        _actions.Player.Disable();
    }

    void OnAttack(InputAction.CallbackContext ctx)
    {
        if (Time.time - _lastFireTime < fireCooldown) return;
        _lastFireTime = Time.time;
        Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        HitQuality quality = BeatManager.Instance.GetHitQuality();

        GameObject obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = obj.GetComponent<Bullet>();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        switch (quality)
        {
            case HitQuality.Perfect:
                bullet.speed = perfectSpeed;
                bullet.damage = perfectDamage;
                obj.transform.localScale = Vector3.one * perfectScale;
                if (sr != null) sr.color = perfectColor;
                break;

            case HitQuality.Good:
                bullet.speed = goodSpeed;
                bullet.damage = goodDamage;
                obj.transform.localScale = Vector3.one * goodScale;
                if (sr != null) sr.color = goodColor;
                break;

            case HitQuality.Miss:
                bullet.speed = missSpeed;
                bullet.damage = missDamage;
                obj.transform.localScale = Vector3.one * missScale;
                if (sr != null) sr.color = missColor;
                break;
        }

        bullet.Init(firePoint.up);
        ScoreManager.Instance.AddScore(quality);
        beatIndicator?.NotifyShot(quality);
    }
}