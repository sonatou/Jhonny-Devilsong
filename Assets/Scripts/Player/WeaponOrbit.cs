using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponOrbit : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 0.6f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 orbitOffset = new Vector2(0f, -0.2f);

    private Transform _player;

    void Awake()
    {
        _player = transform.parent;
    }

    void Update()
    {
        RotateAroundPlayer();
    }

    void RotateAroundPlayer()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 center = (Vector2)_player.position + orbitOffset;
        Vector2 direction = ((Vector2)mouseWorld - center).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = center + new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public Vector2 GetFireDirection()
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector2.up;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
    }
}