using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PursuitBehavior : EnemyBase
{
    public float maxSpeed = 4f;
    public float maxForce = 8f;
    public float predictionFactor = 0.5f;


    protected override void Start()
    {
        base.Start();
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
        Vector2 desired = (predictedPos - _rb.position).normalized * maxSpeed * MusicSpeedMultiplier;
        Vector2 steering = desired - _rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity + steering * Time.fixedDeltaTime, maxSpeed * MusicSpeedMultiplier);

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle );
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