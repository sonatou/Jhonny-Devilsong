using UnityEngine;
using UnityEngine.UI;

public class BeatIndicatorFragment : MonoBehaviour
{
    private RectTransform _rt;
    private Image _image;
    private float _startX;
    private float _speed;

    public void Init(float startX, Color color, float speed)
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _startX = startX;
        _speed = speed;

        _rt.anchoredPosition = new Vector2(startX, 0f);
        _image.color = color;
    }

    void Update()
    {
        _rt.anchoredPosition += Vector2.left * _speed * Time.deltaTime;

        if (_rt.anchoredPosition.x < -(_startX + 20f))
            Destroy(gameObject);
    }
}