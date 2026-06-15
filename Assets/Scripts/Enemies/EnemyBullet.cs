using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float lifetime = 4f;

    private Vector2 _direction;

    public void Init(Vector2 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void FixedUpdate()
    {
        transform.Translate(_direction * speed * Time.fixedDeltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}