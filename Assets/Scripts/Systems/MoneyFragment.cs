using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoneyFragment : MonoBehaviour
{
    [SerializeField] private int value = 10;

    [Header("Lancamento")]
    [SerializeField] private Vector2 launchForce = new Vector2(3f, 6f);
    [SerializeField] private float rotationForce = 2f;

    [Header("Coleta")]
    [SerializeField] private float pickupDelay = 0.4f;
    [SerializeField] private float attractionRadius = 2.5f;
    [SerializeField] private float attractionSpeed = 14f;
    [SerializeField] private float pickupRadius = 0.5f;
    [SerializeField] private ParticleSystem pickupVFX;
    [SerializeField] private EventReference pickupSound;

    private Rigidbody2D _rb;
    private Transform _player;
    private float _spawnTime;
    private bool _collected;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawnTime = Time.time;
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _player = player.transform;

        Vector2 direction = Random.insideUnitCircle.normalized;
        _rb.AddForce(direction * Random.Range(launchForce.x, launchForce.y), ForceMode2D.Impulse);
        _rb.AddTorque(Random.Range(-rotationForce, rotationForce), ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (_collected || _player == null || Time.time - _spawnTime < pickupDelay) return;

        Vector2 target = _player.position;
        float sqrDistance = (target - _rb.position).sqrMagnitude;

        if (sqrDistance <= pickupRadius * pickupRadius)
        {
            Collect();
            return;
        }

        if (sqrDistance <= attractionRadius * attractionRadius)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.MovePosition(Vector2.MoveTowards(_rb.position, target, attractionSpeed * Time.fixedDeltaTime));
        }
    }

    void Collect()
    {
        _collected = true;
        int amount = Mathf.RoundToInt(value * PlayerStats.MoneyMultiplier);
        Wallet.Add(amount);
        if (!pickupSound.IsNull)
            RuntimeManager.PlayOneShot(pickupSound);
        PopupSpawner.Instance?.ShowMoney(amount, transform.position);

        if (pickupVFX != null)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
