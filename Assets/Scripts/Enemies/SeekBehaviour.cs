using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SeekBehavior : MonoBehaviour
{
    public float maxSpeed = 4f;
    public float maxForce = 8f;

    private Rigidbody2D rb;
    public Transform target;

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

        Vector2 desired = ((Vector2)target.position - rb.position).normalized * maxSpeed;
        Vector2 steering = desired - rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed);
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}