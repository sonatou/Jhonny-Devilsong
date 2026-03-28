using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FleeBehavior : MonoBehaviour
{
    public float maxSpeed = 4f;
    public float maxForce = 8f;
    public float panicRadius = 3f;

    private Rigidbody2D rb;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 toTarget = (Vector2)target.position - rb.position;

        if (toTarget.magnitude > panicRadius)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 desired = -toTarget.normalized * maxSpeed;
        Vector2 steering = desired - rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, panicRadius);
    }
}