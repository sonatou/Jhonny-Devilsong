using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyTelegraph : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1f, 0.95f, 0.45f);
    [SerializeField] private float blinkSpeed = 18f;

    private EnemyBase _enemy;
    private Color _baseColor;
    private float _timer;

    void Awake()
    {
        _enemy = GetComponent<EnemyBase>();
    }

    void Start()
    {
        _baseColor = _enemy.Body.color;
    }

    public void Flash(float duration)
    {
        _timer = duration;
    }

    void LateUpdate()
    {
        if (_timer <= 0f) return;

        _timer -= Time.deltaTime;
        float blink = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        _enemy.Body.color = _timer > 0f ? Color.Lerp(_baseColor, color, blink) : _baseColor;
    }
}
