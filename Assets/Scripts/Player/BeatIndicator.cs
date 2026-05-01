using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private RectTransform bar;
    [SerializeField] private Image hitZoneImage;
    [SerializeField] private Image perfectZoneImage;

    [Header("Configuração")]
    [SerializeField] private int fragmentsAhead = 3;
    [SerializeField] private Color fragmentColor = Color.white;
    [SerializeField] private Color goodZoneColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color perfectZoneColor = new Color(1f, 0.9f, 0f, 0.4f);

    private float _barWidth;
    private float _hitLineX;

    void Start()
    {
        _barWidth = bar.rect.width;
        _hitLineX = -_barWidth * 0.5f + _barWidth * 0.2f;

        SetupHitZone();
        BeatManager.Instance.OnBeat += SpawnFragment;
    }

    void OnDestroy()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= SpawnFragment;
    }

    void SetupHitZone()
    {
        if (hitZoneImage == null) return;

        hitZoneImage.color = goodZoneColor;
        hitZoneImage.rectTransform.sizeDelta = new Vector2(30f, bar.rect.height);
        hitZoneImage.rectTransform.anchoredPosition = new Vector2(_hitLineX, 0f);

        if (perfectZoneImage == null) return;

        perfectZoneImage.color = perfectZoneColor;
        perfectZoneImage.rectTransform.sizeDelta = new Vector2(10f, bar.rect.height);
        perfectZoneImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    void SpawnFragment()
    {
        float beatInterval = BeatManager.Instance.BeatInterval;
        float fragmentSpeed = (_barWidth * 0.8f) / beatInterval;

        for (int i = 1; i <= fragmentsAhead; i++)
        {
            float startX = _barWidth * 0.5f - 10f;

            if (i > 1)
                startX = _hitLineX + fragmentSpeed * beatInterval * i;

            if (startX > _barWidth * 0.5f + 20f) continue;

            GameObject obj = Instantiate(fragmentPrefab, bar);
            obj.GetComponent<BeatIndicatorFragment>().Init(startX, fragmentColor, fragmentSpeed);
        }
    }
}