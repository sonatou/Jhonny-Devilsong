using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(EnemyBase))]
public class EnemyBeatLunge : MonoBehaviour
{
    [SerializeField] private EnemyTelegraph telegraph;
    [SerializeField] private float range = 6f;
    [SerializeField] private int beatsBetween = 4;
    [SerializeField] private float lungeSpeed = 14f;
    [SerializeField] private float lungeDuration = 0.25f;

    private EnemyBase _enemy;
    private Rigidbody2D _rb;
    private Transform _target;
    private int _beatsSinceLunge;
    private bool _windingUp;
    private Vector2 _lungeVelocity;
    private float _lungeTimer;

    void Awake()
    {
        _enemy = GetComponent<EnemyBase>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _target = player.transform;

        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnDestroy()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= OnBeat;
    }

    void OnBeat()
    {
        if (_target == null) return;

        _beatsSinceLunge++;

        if (_windingUp)
        {
            _windingUp = false;
            _beatsSinceLunge = 0;
            _lungeVelocity = ((Vector2)_target.position - _rb.position).normalized * lungeSpeed * _enemy.SpeedMultiplier;
            _lungeTimer = lungeDuration;
            return;
        }

        if (_beatsSinceLunge < beatsBetween) return;
        if (Vector2.Distance(_target.position, _rb.position) > range) return;

        _windingUp = true;
        if (telegraph != null) telegraph.Flash(BeatManager.Instance.BeatInterval);
    }

    void FixedUpdate()
    {
        if (_windingUp)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        if (_lungeTimer <= 0f) return;

        _rb.linearVelocity = _lungeVelocity;
        _lungeTimer -= Time.fixedDeltaTime;
    }
}
