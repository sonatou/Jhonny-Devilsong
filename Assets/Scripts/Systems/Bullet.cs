using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed  = 10f;
    [SerializeField] public int   damage = 1;
    [SerializeField] private float lifetime = 3f;

    private Vector2    _direction;
    private HitQuality _quality;

    public void Init(Vector2 direction, HitQuality quality)
    {
        _direction = direction.normalized;
        _quality   = quality;
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
        {
            health.TakeDamage(damage);
            ScoreManager.Instance?.RegisterHit(_quality);
        }

        Destroy(gameObject);
    }
}
