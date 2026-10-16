using UnityEngine;
using UnityEngine.UI;

public class DashIndicator : MonoBehaviour
{
    [SerializeField] private PlayerDash dash;
    [SerializeField] private Image ring;
    [SerializeField] private Color chargingColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color readyColor = new Color(0.35f, 0.65f, 1f, 0.85f);
    [SerializeField] private float readyPunch = 1.4f;
    [SerializeField] private float punchDuration = 0.2f;

    private Vector3 _baseScale;
    private bool _wasReady = true;
    private float _punchTimer;

    void Awake()
    {
        _baseScale = ring.transform.localScale;
    }

    void LateUpdate()
    {
        bool unlocked = PlayerStats.DashUnlocked;
        ring.enabled = unlocked;
        if (!unlocked) return;

        float progress = dash.CooldownProgress;
        bool ready = progress >= 1f;
        if (ready && !_wasReady) _punchTimer = punchDuration;
        _wasReady = ready;

        ring.fillAmount = progress;
        ring.color = ready ? readyColor : chargingColor;

        _punchTimer = Mathf.Max(0f, _punchTimer - Time.deltaTime);
        float punch = punchDuration > 0f ? _punchTimer / punchDuration : 0f;
        ring.transform.localScale = _baseScale * Mathf.Lerp(1f, readyPunch, punch);
    }
}
