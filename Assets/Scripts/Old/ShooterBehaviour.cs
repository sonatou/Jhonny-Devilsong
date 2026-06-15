using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShooterBehaviour : EnemyBase
{
    [Header("Distâncias")]
    public float safeDistance = 6f;
    public float panicRadius = 3f;

    [Header("Movimento")]
    public float moveSpeed = 3f;

    [Header("Tiro")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 2f;
    [SerializeField] private GameObject sniper;

    private float _lastFireTime = -Mathf.Infinity;

    private enum State { Approach, Combat, Flee }
    private State _state;

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        if (_target == null) return;

        float distance = Vector2.Distance(_rb.position, _target.position);

        UpdateState(distance);

        switch (_state)
        {
            case State.Approach:
                MoveTowards(_target.position);
                break;

            case State.Combat:
                _rb.linearVelocity = Vector2.zero;
                TryShoot();
                break;

            case State.Flee:
                MoveAwayFrom(_target.position);
                break;
        }

        RotateTowardsTarget();
    }

    void UpdateState(float distance)
    {
        if (distance < panicRadius)
        {
            _state = State.Flee;
            return;
        }

        if (distance > safeDistance)
        {
            _state = State.Approach;
            return;
        }

        _state = State.Combat;
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - _rb.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;
    }

    void MoveAwayFrom(Vector2 target)
    {
        Vector2 direction = (_rb.position - target).normalized;
        _rb.linearVelocity = direction * moveSpeed;
    }

    void TryShoot()
    {
        if (Time.time - _lastFireTime < fireCooldown) return;
        if (bulletPrefab == null || firePoint == null) return;

        _lastFireTime = Time.time;

        Vector2 direction = ((Vector2)_target.position - (Vector2)firePoint.position).normalized;
        GameObject obj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        EnemyBullet bullet = obj.GetComponent<EnemyBullet>();
        if (bullet != null)
            bullet.Init(direction);
    }

    void RotateTowardsTarget()
    {
        if (_target == null) return;
        Vector2 direction = ((Vector2)_target.position - _rb.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sniper.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    protected override void FlipToPlayerPostion()
    {
        if (_target.transform.position.x - transform.position.x < 0)
        {
            spriteRenderer.flipX = true;
            sniper.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            spriteRenderer.flipX = false;
            sniper.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, safeDistance);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, panicRadius);
    }
}