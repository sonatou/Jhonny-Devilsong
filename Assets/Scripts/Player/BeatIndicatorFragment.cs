using UnityEngine;

public class BeatIndicatorFragment : MonoBehaviour
{
    public double BeatTime { get; private set; }

    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f, 0.8f);

    private SpriteRenderer _sr;
    private Transform      _barCenter;
    private float          _speed;
    private float          _hitLineOffset;

    public void Init(double beatTime, float hitLineOffset, Color color, float speed, Transform barCenter)
    {
        _sr            = GetComponent<SpriteRenderer>();
        _barCenter     = barCenter;
        _speed         = speed;
        _hitLineOffset = hitLineOffset;
        BeatTime       = beatTime;

        _sr.color = color;
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();

        double timeUntilBeat = BeatTime - AudioSettings.dspTime;
        if (timeUntilBeat < -0.08f)
            Destroy(gameObject);
    }

    void UpdatePosition()
    {
        if (_barCenter == null) return;

        double timeUntilBeat = BeatTime - AudioSettings.dspTime;
        float  xOffset       = _hitLineOffset + (float)(timeUntilBeat * _speed);

        transform.position = _barCenter.position + new Vector3(xOffset, 0f, 0f);
    }

    public void OnPerfect() => Destroy(gameObject);

    public void OnGood()
    {
        _sr.color = goodColor;
        Destroy(gameObject);
    }
}
