using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public int damage = 1;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject impactVFX;
    [SerializeField] private ParticleSystem hitSlash;

    [Header("Cores do impacto")]
    [SerializeField] private Color perfectColor = Color.yellow;
    [SerializeField] private Color goodColor = Color.white;
    [SerializeField] private Color missColor = Color.gray;

    private Vector2 _direction;
    private HitQuality _quality;

    public void Init(Vector2 direction, HitQuality quality)
    {
        _direction = direction.normalized;
        _quality = quality;

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        transform.Translate(_direction * speed * Time.fixedDeltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        EnemyBase health = other.GetComponent<EnemyBase>();
        if (health != null)
        {
            ComboManager.Instance?.RegisterHit(_quality);
            health.TakeDamage(damage);
            SpawnHitSlash();
        }

        SpawnImpactVFX();
        Destroy(gameObject);
    }

    void SpawnImpactVFX()
    {
        if (impactVFX == null) return;

        GameObject vfx = Instantiate(impactVFX, transform.position, Quaternion.identity);
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        var main = ps.main;

        main.startColor = QualityColor();

        ps.Play();
    }

    void SpawnHitSlash()
    {
        if (hitSlash == null) return;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        ParticleSystem slash = Instantiate(hitSlash, transform.position, Quaternion.Euler(0f, 0f, angle));
        var main = slash.main;
        main.startColor = QualityColor();
        slash.Play();
    }

    Color QualityColor()
    {
        return _quality switch
        {
            HitQuality.Perfect => perfectColor,
            HitQuality.Good => goodColor,
            HitQuality.Miss => missColor,
            _ => Color.white
        };
    }
}