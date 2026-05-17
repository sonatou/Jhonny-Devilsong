using UnityEngine;
using System.Collections.Generic;

public class BeatIndicator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject fragmentPrefab;

    [Header("Referencias")]
    [SerializeField] private Transform      barBackground;
    [SerializeField] private SpriteRenderer hitZone;
    [SerializeField] private SpriteRenderer perfectZone;

    [Header("Configuracao")]
    [SerializeField] private int   fragmentsAhead   = 1;
    [SerializeField] private Color fragmentColor    = Color.white;
    [SerializeField] private Color goodZoneColor    = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color perfectZoneColor = new Color(1f, 0.9f, 0f, 0.4f);

    private float  _barWidth;
    private float  _hitLineOffset;
    private double _lastBeatTime;
    private List<BeatIndicatorFragment> _fragments = new List<BeatIndicatorFragment>();

    void Start()
    {
        _barWidth      = barBackground != null ? barBackground.localScale.x : 2f;
        _hitLineOffset = -_barWidth * 0.5f + _barWidth * 0.2f;

        SetupVisuals();
        BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnDestroy()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= OnBeat;
    }

    void SetupVisuals()
    {
        if (hitZone != null)
        {
            hitZone.color = goodZoneColor;
            hitZone.transform.localPosition = new Vector3(_hitLineOffset, 0f, 0f);
        }

        if (perfectZone != null)
        {
            perfectZone.color = perfectZoneColor;
            perfectZone.transform.localPosition = new Vector3(_hitLineOffset, 0f, 0f);
        }
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

            GameObject obj             = Instantiate(fragmentPrefab);
            BeatIndicatorFragment frag = obj.GetComponent<BeatIndicatorFragment>();
            frag.Init(beatTime, _hitLineOffset, fragmentColor, calculatedSpeed, barBackground, _barWidth);
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
            double timeUntilBeat = f.BeatTime - currentTime;

            // so considera fragmentos que ja estao proximos ou passando da hit zone
            if (timeUntilBeat > 0.3f) continue;

            double diff = System.Math.Abs(timeUntilBeat);
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
