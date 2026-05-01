using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrivalBehavior : MonoBehaviour
{
    public float maxSpeed = 4f;
    public float slowRadius = 3f;

    private Rigidbody2D _rb;
    private Transform _target;
    private bool _touching = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _target = player.transform;
    }

    void FixedUpdate()
    {
        if (_target == null || _touching) return;

        Vector2 toTarget = (Vector2)_target.position - _rb.position;
        float distance = toTarget.magnitude;

        float targetSpeed = distance < slowRadius
            ? maxSpeed * (distance / slowRadius)
            : maxSpeed;

        _rb.linearVelocity = toTarget.normalized * targetSpeed;

        float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _touching = true;
            _rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            _touching = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, slowRadius);
    }
}