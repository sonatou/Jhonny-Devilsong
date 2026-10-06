using UnityEngine;

public class SoloSmokeTrail : MonoBehaviour
{
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private float maxRatePerUnit = 12f;
    [SerializeField] private int noteBurst = 6;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (SoloAnalyzer.Instance != null)
            SoloAnalyzer.Instance.OnNote += OnNote;
    }

    void OnDestroy()
    {
        if (SoloAnalyzer.Instance != null)
            SoloAnalyzer.Instance.OnNote -= OnNote;
    }

    void Update()
    {
        float energy = SoloAnalyzer.Instance != null ? SoloAnalyzer.Instance.Energy : 0f;
        var emission = smoke.emission;
        emission.rateOverDistance = maxRatePerUnit * energy;
    }

    void OnNote()
    {
        if (_rb.linearVelocity.magnitude < minMoveSpeed) return;
        smoke.Emit(noteBurst);
    }
}
