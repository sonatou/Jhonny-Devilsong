using UnityEngine;
using UnityEngine.UI;

public class BeatVisualizer : MonoBehaviour
{
    public Image indicator;
    public Color onColor = Color.white;
    public Color offColor = new Color(1f, 1f, 1f, 0.2f);
    public float flashDuration = 0.1f;

    private float _flashTimer = 0f;

    void Start()
    {
        BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= OnBeat;
    }

    void Update()
    {
        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            indicator.color = onColor;
        }
        else
        {
            indicator.color = offColor;
        }
    }

    void OnBeat()
    {
        _flashTimer = flashDuration;
    }
}