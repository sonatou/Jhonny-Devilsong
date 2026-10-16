using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FleeBehavior : EnemyBase
{
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxForce = 8f;
    [SerializeField] private float panicRadius = 3f;
    [SerializeField] private float idleMinDuration = 1f;
    [SerializeField] private float idleMaxDuration = 3f;
    [SerializeField] private float roamDuration = 1.5f;

    [Header("Obstaculos")]
    [SerializeField] private LayerMask obstacleMask = 1;
    [SerializeField] private float obstacleCheckDistance = 1.2f;
    [SerializeField] private float escapeCheckDistance = 2.5f;
    [SerializeField] private int obstacleSamples = 16;
    [SerializeField] private float escapeCommitTime = 0.5f;

    private Vector2 _roamDirection;
    private bool _isRoaming = false;
    private ContactFilter2D _obstacleFilter;
    private readonly RaycastHit2D[] _obstacleHits = new RaycastHit2D[4];
    private Vector2 _escapeDirection;
    private float _escapeUntil;

    protected override void Start()
    {
        base.Start();

        _obstacleFilter.useTriggers = false;
        _obstacleFilter.SetLayerMask(obstacleMask);
        StartCoroutine(RoamRoutine());
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
        if (_target == null) return;

        Vector2 toTarget = (Vector2)_target.position - _rb.position;

        if (toTarget.magnitude <= panicRadius)
        {
            Flee(toTarget);
            return;
        }

        if (_isRoaming)
            Roam();
        else
            _rb.linearVelocity = Vector2.zero;
        if (_rb.linearVelocity.x != 0)
            spriteRenderer.flipY = _rb.linearVelocity.x < 0;
    }

    void Flee(Vector2 toTarget)
    {
        Vector2 desired = AvoidObstacles(-toTarget.normalized) * maxSpeed * MusicSpeedMultiplier;
        Vector2 steering = desired - _rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);
        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed * MusicSpeedMultiplier);

        UpdateRotation();
    }

    void Roam()
    {
        _roamDirection = AvoidObstacles(_roamDirection);
        _rb.linearVelocity = _roamDirection * (maxSpeed * MusicSpeedMultiplier * 0.5f);
        UpdateRotation();
    }

    Vector2 AvoidObstacles(Vector2 direction)
    {
        if (!IsBlocked(direction, obstacleCheckDistance)) return direction;
        if (Time.time < _escapeUntil && !IsBlocked(_escapeDirection, obstacleCheckDistance)) return _escapeDirection;

        if (!TryFindDirection(direction, escapeCheckDistance, out _escapeDirection)
            && !TryFindDirection(direction, obstacleCheckDistance, out _escapeDirection))
            _escapeDirection = -direction;

        _escapeUntil = Time.time + escapeCommitTime;
        return _escapeDirection;
    }

    bool TryFindDirection(Vector2 direction, float distance, out Vector2 best)
    {
        best = Vector2.zero;
        float bestScore = float.MinValue;

        for (int i = 0; i < obstacleSamples; i++)
        {
            float angle = i * Mathf.PI * 2f / obstacleSamples;
            Vector2 candidate = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            float score = Vector2.Dot(candidate, direction);

            if (score > bestScore && !IsBlocked(candidate, distance))
            {
                best = candidate;
                bestScore = score;
            }
        }

        return bestScore > float.MinValue;
    }

    bool IsBlocked(Vector2 direction, float distance)
    {
        int count = Physics2D.Raycast(_rb.position, direction, _obstacleFilter, _obstacleHits, distance);
        for (int i = 0; i < count; i++)
        {
            if (_obstacleHits[i].rigidbody == null) return true;
        }

        return false;
    }

    void UpdateRotation()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            _isRoaming = false;
            yield return new WaitForSeconds(Random.Range(idleMinDuration, idleMaxDuration));

            _roamDirection = Random.insideUnitCircle.normalized;
            _isRoaming = true;
            yield return new WaitForSeconds(roamDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, panicRadius);
    }
}