using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FleeBehavior : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxForce = 8f;
    [SerializeField] private float panicRadius = 3f;
    [SerializeField] private float idleMinDuration = 1f;
    [SerializeField] private float idleMaxDuration = 3f;
    [SerializeField] private float roamDuration = 1.5f;

    private Rigidbody2D _rb;
    private Transform _target;
    private Vector2 _roamDirection;
    private bool _isRoaming = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _target = player.transform;

        StartCoroutine(RoamRoutine());
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
    }

    void Flee(Vector2 toTarget)
    {
        Vector2 desired = -toTarget.normalized * maxSpeed;
        Vector2 steering = desired - _rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);
        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed);
        UpdateRotation();
    }

    void Roam()
    {
        _rb.linearVelocity = _roamDirection * (maxSpeed * 0.5f);
        UpdateRotation();
    }

    void UpdateRotation()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
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