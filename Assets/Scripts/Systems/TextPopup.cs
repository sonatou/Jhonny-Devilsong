using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextPopup : MonoBehaviour
{
    [Header("Tempo")]
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField, Range(0f, 0.95f)] private float fadeStart = 0.3f;

    [Header("Movimento")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 30f);
    [SerializeField] private float sidewaysSpeed = 90f;
    [SerializeField] private float upwardSpeed = 120f;
    [SerializeField] private float upwardSpread = 100f;
    [SerializeField] private float gravity = 480f;
    [SerializeField, Range(0f, 180f)] private float maxRotation = 45f;

    private TextMeshProUGUI _label;
    private RectTransform _rect;
    private Vector2 _origin;
    private Vector2 _velocity;
    private float _age;
    private float _alpha = 1f;

    void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
        _rect = _label.rectTransform;
        _velocity = new Vector2(Random.Range(-sidewaysSpeed, sidewaysSpeed), upwardSpeed + Random.Range(-upwardSpread, upwardSpread));
    }

    public void Initialize(string text, Color color, Vector3 worldPosition, Camera worldCamera)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_rect.parent, screenPosition, canvasCamera, out _origin);

        _origin += spawnOffset;
        _rect.anchoredPosition = _origin;
        _label.text = text;
        _label.color = color;
        _alpha = color.a;
    }

    void Update()
    {
        _age += Time.deltaTime;
        if (_age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        _rect.anchoredPosition = _origin + _velocity * _age + Vector2.down * (0.5f * gravity * _age * _age);
        float direction = sidewaysSpeed == 0f ? 0f : _velocity.x / Mathf.Abs(sidewaysSpeed);
        _rect.localRotation = Quaternion.Euler(0f, 0f, -direction * maxRotation * (_age / lifetime));
        float fade = Mathf.InverseLerp(lifetime * fadeStart, lifetime, _age);
        _label.alpha = _alpha * (1f - Mathf.SmoothStep(0f, 1f, fade));
    }
}
