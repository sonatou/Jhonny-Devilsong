using UnityEngine;

[DefaultExecutionOrder(100)]
public class EnemyExpel : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private float _duration;
    private float _timer;

    public void Init(Vector2 offset, float duration)
    {
        _rb = GetComponent<Rigidbody2D>();
        _duration = Mathf.Max(0.01f, duration);
        _velocity = offset / _duration;
        _timer = _duration;
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _velocity * (2f * _timer / _duration);
        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0f) Destroy(this);
    }
}
