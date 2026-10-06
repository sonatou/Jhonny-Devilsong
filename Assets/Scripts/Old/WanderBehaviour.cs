using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WanderBehavior : EnemyBase
{
    public float maxSpeed = 3f;
    public float circleDistance = 2f;
    public float circleRadius = 1f;
    public float angleChange = 40f;

    
    private float _wanderAngle = 0f;
    private Vector2 _lastDirection;

    protected override void Start()
    {
        base.Start();

        _wanderAngle = Random.Range(0f, 360f);
        _lastDirection = Random.insideUnitCircle.normalized;
    }

    protected override void FlipToPlayerPostion()
    {
        if (_target.transform.position.x - transform.position.x < 0)
        {
            //spriteRenderer.flipY = true;
        }
        else
        {
            //spriteRenderer.flipY = false;
        }
    }

    void FixedUpdate()
    {
        _wanderAngle += Random.Range(-angleChange, angleChange) * Time.fixedDeltaTime;

        // Mantém a última direção válida para não travar ao bater na parede
        Vector2 velocity = _rb.linearVelocity;
        if (velocity.sqrMagnitude > 0.01f)
            _lastDirection = velocity.normalized;

        Vector2 circleCenter = _rb.position + _lastDirection * circleDistance;

        Vector2 displacement = new Vector2(
            Mathf.Cos(_wanderAngle * Mathf.Deg2Rad),
            Mathf.Sin(_wanderAngle * Mathf.Deg2Rad)) * circleRadius;

        Vector2 wanderTarget = circleCenter + displacement;

        Vector2 desired = (wanderTarget - _rb.position).normalized * maxSpeed * MusicSpeedMultiplier;
        _rb.linearVelocity = desired;

        float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        spriteRenderer.flipY = _rb.linearVelocity.x < 0;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Ao bater em qualquer coisa, inverte e gira a direção
        _lastDirection = -_lastDirection;
        _wanderAngle += 180f + Random.Range(-60f, 60f);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 circleCenter = (Vector2)transform.position + _lastDirection * circleDistance;

        Gizmos.color = new Color(1f, 0.8f, 0f, 0.5f);
        int seg = 24;
        for (int i = 0; i < seg; i++)
        {
            float a1 = i * Mathf.PI * 2f / seg;
            float a2 = (i + 1) * Mathf.PI * 2f / seg;
            Gizmos.DrawLine(
                circleCenter + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * circleRadius,
                circleCenter + new Vector2(Mathf.Cos(a2), Mathf.Sin(a2)) * circleRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, circleCenter);
    }
}