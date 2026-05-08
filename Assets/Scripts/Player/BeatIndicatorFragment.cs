using UnityEngine;
using UnityEngine.UI;

public class BeatIndicatorFragment : MonoBehaviour
{
    public double BeatTime { get; private set; }

    private RectTransform _rt;
    private Image _image;
    private float _hitLineX;
    private float _speed;

    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f, 0.8f);

    public void Init(double beatTime, float hitLineX, Color color, float speed)
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _hitLineX = hitLineX;
        _speed = speed;
        BeatTime = beatTime;

        _image.color = color;
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();

        if (AudioSettings.dspTime > BeatTime + 0.3f)
            Destroy(gameObject);
    }

    void UpdatePosition()
    {
        double timeUntilBeat = BeatTime - AudioSettings.dspTime;
        _rt.anchoredPosition = new Vector2(_hitLineX + (float)(timeUntilBeat * _speed), 0f);
    }

    public void OnPerfect() => Destroy(gameObject);

    public void OnGood()
    {
        _image.color = goodColor;
        Destroy(gameObject);
    }
}