using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponOrbit : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 0.6f;
    [SerializeField] private Transform firePoint;
    //[SerializeField] private Vector2 orbitOffset = new Vector2(0f, -0.2f);

    [Header("Recuo no Perfect")]
    [SerializeField] private Transform weaponSprite;
    [SerializeField] private Vector3 punchScale = new Vector3(1.3f, 0.8f, 1f);
    [SerializeField] private float recoilDistance = 0.15f;
    [SerializeField] private float punchDuration = 0.12f;

    private Vector3 originalScale;
    private Vector3 _spriteBaseScale;
    private float _punchTimer;

    private Transform _player;

    void Awake()
    {
        _player = transform.parent;
        originalScale = transform.localScale;
        if (weaponSprite != null) _spriteBaseScale = weaponSprite.localScale;
    }

    void Update()
    {
        RotateAroundPlayer();
    }

    public void Punch()
    {
        _punchTimer = punchDuration;
    }

    void RotateAroundPlayer()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float punch = punchDuration > 0f ? _punchTimer / punchDuration : 0f;
        _punchTimer = Mathf.Max(0f, _punchTimer - Time.deltaTime);
        float radius = orbitRadius - recoilDistance * punch;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 center = (Vector2)_player.position;
        Vector2 direction = ((Vector2)mouseWorld - center).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = center + new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip do sprite
        Vector3 scale = originalScale;
        scale.y *= direction.x < 0 ? -1 : 1;
        transform.localScale = scale;

        if (weaponSprite != null)
            weaponSprite.localScale = Vector3.Scale(_spriteBaseScale, Vector3.Lerp(Vector3.one, punchScale, punch));
    }

    public Vector2 GetFireDirection()
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector2.up;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
    }
}
