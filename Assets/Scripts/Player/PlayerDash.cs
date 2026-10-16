using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float distance = 2.5f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float cooldown = 0.6f;

    [Header("Ritmo")]
    [SerializeField] private float perfectMultiplier = 1.6f;
    [SerializeField] private float goodMultiplier = 1.25f;

    [Header("Som")]
    [SerializeField] private EventReference dashSound;

    [Header("Referencias")]
    [SerializeField] private PlayerHealth health;
    [SerializeField] private WeaponOrbit weaponOrbit;

    private Rigidbody2D _rb;
    private InputSystem_Actions _actions;
    private Vector2 _velocity;
    private float _dashTimer;
    private float _lastDashTime = -Mathf.Infinity;

    public float CooldownProgress => Mathf.Clamp01((Time.time - _lastDashTime) / (cooldown * PlayerStats.DashCooldownMultiplier));

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Sprint.performed += OnDash;
    }

    void OnDisable()
    {
        _actions.Player.Sprint.performed -= OnDash;
        _actions.Player.Disable();
        EndDash();
    }

    void OnDash(InputAction.CallbackContext ctx)
    {
        if (!PlayerStats.DashUnlocked || PauseMenu.IsPaused || SkillTreeUI.IsOpen || _dashTimer > 0f) return;
        if (Time.time - _lastDashTime < cooldown * PlayerStats.DashCooldownMultiplier) return;

        Vector2 direction = _actions.Player.Move.ReadValue<Vector2>();
        if (direction.sqrMagnitude < 0.01f && weaponOrbit != null)
            direction = weaponOrbit.GetFireDirection();
        if (direction.sqrMagnitude < 0.01f) return;

        float total = (distance + PlayerStats.DashDistanceBonus) * RhythmMultiplier();
        _velocity = direction.normalized * total / duration;
        _dashTimer = duration;
        _lastDashTime = Time.time;
        if (!dashSound.IsNull)
            RuntimeManager.PlayOneShot(dashSound);
        if (health != null) health.SetShielded(true);
    }

    float RhythmMultiplier()
    {
        if (BeatManager.Instance == null || !BeatManager.Instance.IsPlaying) return 1f;

        return BeatManager.Instance.GetHitQuality() switch
        {
            HitQuality.Perfect => perfectMultiplier,
            HitQuality.Good    => goodMultiplier,
            _                  => 1f
        };
    }

    void FixedUpdate()
    {
        if (_dashTimer <= 0f) return;

        _rb.linearVelocity = _velocity;
        _dashTimer -= Time.fixedDeltaTime;
        if (_dashTimer <= 0f) EndDash();
    }

    void EndDash()
    {
        _dashTimer = 0f;
        if (health != null) health.SetShielded(false);
    }
}
