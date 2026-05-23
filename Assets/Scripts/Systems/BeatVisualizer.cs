using UnityEngine;
using UnityEngine.UI;

public class BeatVisualizer : MonoBehaviour
{
    [SerializeField] private Image indicator;
    [SerializeField] private Color onColor      = Color.white;
    [SerializeField] private Color offColor     = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private float flashDuration = 0.1f;

    private float _flashTimer = 0f;

    void OnEnable()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        if (BeatManager.Instance != null)
            BeatManager.Instance.OnBeat -= OnBeat;
    }

    void Update()
    {
        if (indicator == null) return;

        if (_flashTimer > 0f)
        {
            _flashTimer    -= Time.deltaTime;
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
