using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PursuitBehavior : MonoBehaviour
{
    public float maxSpeed = 4f;
    public float maxForce = 8f;
    public float predictionFactor = 0.5f;

    private Rigidbody2D _rb;
    private Rigidbody2D _targetRb;
    private Transform _target;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _target = player.transform;
            _targetRb = player.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        if (_target == null) return;

        Vector2 toTarget = (Vector2)_target.position - _rb.position;
        float distance = toTarget.magnitude;

        // Tempo de predição proporcional à distância
        float speed = _rb.linearVelocity.magnitude;
        float predictionTime = speed > 0.01f
            ? Mathf.Min(distance / speed * predictionFactor, 1f)
            : predictionFactor;

        // Posição futura prevista do jogador
        Vector2 targetVelocity = _targetRb != null ? _targetRb.linearVelocity : Vector2.zero;
        Vector2 predictedPos = (Vector2)_target.position + targetVelocity * predictionTime;

        // Seek para a posição prevista
        Vector2 desired = (predictedPos - _rb.position).normalized * maxSpeed;
        Vector2 steering = desired - _rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed);

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_target == null) return;

        // Posição prevista
        Vector2 targetVelocity = _targetRb != null ? _targetRb.linearVelocity : Vector2.zero;
        float predictionTime = predictionFactor;
        Vector2 predicted = (Vector2)_target.position + targetVelocity * predictionTime;

        // Linha até a posição atual do jogador
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawLine(transform.position, _target.position);

        // Linha até a posição prevista
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, predicted);
        Gizmos.DrawWireSphere(predicted, 0.2f);
    }
}