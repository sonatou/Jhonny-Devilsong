using FMODUnity;
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
    [SerializeField] private WeaponOrbit _weaponOrbit;

    [Header("Cooldown")]
    [SerializeField] private float fireCooldown = 0.3f;

    [Header("Perfect")]
    [SerializeField] private float perfectSpeed = 14f;
    [SerializeField] private int perfectDamage = 3;
    [SerializeField] private float perfectScale = 1.5f;
    [SerializeField] private Sprite perfectSprite = null;
    [SerializeField] private float perfectShake = 0.2f;
    [SerializeField] private ParticleSystem perfectSparks;

    [Header("Good")]
    [SerializeField] private float goodSpeed = 10f;
    [SerializeField] private int goodDamage = 2;
    [SerializeField] private float goodScale = 1f;
    [SerializeField] private Sprite goodSprite = null;

    [Header("Miss")]
    [SerializeField] private float missSpeed = 7f;
    [SerializeField] private int missDamage = 1;
    [SerializeField] private float missScale = 0.6f;
    [SerializeField] private Sprite missSprite = null;

    [Header("Som")]
    [SerializeField] private EventReference shotSound;

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
        if (PauseMenu.IsPaused || SkillTreeUI.IsOpen) return;
        if (Time.time - _lastFireTime < fireCooldown) return;
        _lastFireTime = Time.time;
        Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        HitQuality quality = BeatManager.Instance.GetHitQuality();

        if (quality == HitQuality.Miss)
            ComboManager.Instance?.ResetCombo();
        if (quality == HitQuality.Miss)
            BeatManager.Instance?.OnMiss();

        GameObject obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = obj.GetComponent<Bullet>();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        switch (quality)
        {
            case HitQuality.Perfect:
                bullet.speed = perfectSpeed;
                bullet.damage = PlayerStats.ApplyDamage(perfectDamage);
                obj.transform.localScale = Vector3.one * perfectScale;
                if (sr != null && perfectSprite != null) sr.sprite = perfectSprite;
                break;

            case HitQuality.Good:
                bullet.speed = goodSpeed;
                bullet.damage = PlayerStats.ApplyDamage(goodDamage);
                obj.transform.localScale = Vector3.one * goodScale;
                if (sr != null && goodSprite != null) sr.sprite = goodSprite;
                break;

            case HitQuality.Miss:
                bullet.speed = missSpeed;
                bullet.damage = PlayerStats.ApplyDamage(missDamage);
                obj.transform.localScale = Vector3.one * missScale;
                if (sr != null && missSprite != null) sr.sprite = missSprite;
                break;
        }

        Vector2 fireDir = _weaponOrbit != null ? _weaponOrbit.GetFireDirection() : firePoint.up;
        bullet.Init(fireDir, quality);
        PlayShotSound(quality);

        if (beatIndicator == null)
            beatIndicator = GetComponentInChildren<BeatIndicator>();
        beatIndicator?.NotifyShot(quality);

        if (quality == HitQuality.Perfect)
            PlayPerfectFeedback();
    }

    void PlayShotSound(HitQuality quality)
    {
        if (shotSound.IsNull) return;

        FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(shotSound);
        instance.setParameterByNameWithLabel("Quality", quality.ToString());
        instance.start();
        instance.release();
    }

    void PlayPerfectFeedback()
    {
        CameraFeedback.Instance?.Shake(perfectShake);
        _weaponOrbit?.Punch();
        if (perfectSparks != null)
            Instantiate(perfectSparks, firePoint.position, firePoint.rotation);
    }
}