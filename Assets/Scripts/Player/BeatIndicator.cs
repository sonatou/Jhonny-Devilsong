using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private RectTransform bar;
    [SerializeField] private Image hitZoneImage;
    [SerializeField] private Image perfectZoneImage;

    [Header("Configuracao")]
    [SerializeField] private int   fragmentsAhead   = 3;
    [SerializeField] private Color fragmentColor    = Color.white;
    [SerializeField] private Color goodZoneColor    = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color perfectZoneColor = new Color(1f, 0.9f, 0f, 0.4f);

    private float _barWidth;
    private float _hitLineX;
    private double _lastBeatTime;
    private List<BeatIndicatorFragment> _fragments = new List<BeatIndicatorFragment>();

    void Start()
    {
        _barWidth = bar.rect.width;
        _hitLineX = -_barWidth * 0.5f + _barWidth * 0.2f;

        SetupHitZone();
        BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnDestroy()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= OnBeat;
    }

    void SetupHitZone()
    {
        if (hitZoneImage == null) return;

        hitZoneImage.color = goodZoneColor;
        hitZoneImage.rectTransform.sizeDelta        = new Vector2(30f, bar.rect.height);
        hitZoneImage.rectTransform.anchoredPosition = new Vector2(_hitLineX, 0f);

        if (perfectZoneImage == null) return;

        perfectZoneImage.color = perfectZoneColor;
        perfectZoneImage.rectTransform.sizeDelta        = new Vector2(10f, bar.rect.height);
        perfectZoneImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    void OnBeat()
    {
        _lastBeatTime = AudioSettings.dspTime;
        float beatInterval    = BeatManager.Instance.BeatInterval;
        float calculatedSpeed = (_barWidth * 0.8f) / beatInterval;

        _fragments.RemoveAll(f => f == null);

        for (int i = 1; i <= fragmentsAhead; i++)
        {
            double beatTime = _lastBeatTime + beatInterval * i;

            GameObject obj             = Instantiate(fragmentPrefab, bar);
            BeatIndicatorFragment frag = obj.GetComponent<BeatIndicatorFragment>();
            frag.Init(beatTime, _hitLineX, fragmentColor, calculatedSpeed);
            _fragments.Add(frag);
        }
    }

    public void NotifyShot(HitQuality quality)
    {
        _fragments.RemoveAll(f => f == null);

        double currentTime            = AudioSettings.dspTime;
        BeatIndicatorFragment closest = null;
        double closestTimeDiff        = double.MaxValue;

        foreach (var f in _fragments)
        {
            double diff = System.Math.Abs(f.BeatTime - currentTime);
            if (diff < closestTimeDiff)
            {
                closestTimeDiff = diff;
                closest         = f;
            }
        }

        if (closest == null) return;

        switch (quality)
        {
            case HitQuality.Perfect:
                closest.OnPerfect();
                _fragments.Remove(closest);
                break;
            case HitQuality.Good:
                closest.OnGood();
                _fragments.Remove(closest);
                break;
            case HitQuality.Miss:
                break;
        }
    }
}
