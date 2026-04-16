using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 3f;

    private Vector2 _direction;

    public void Init(Vector2 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        transform.Translate(_direction * speed * Time.fixedDeltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health != null)
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}