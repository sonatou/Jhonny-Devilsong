using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown beatBarDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider latencySlider;
    [SerializeField] private TMP_Text latencyLabel;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private readonly List<Resolution> _resolutions = new List<Resolution>();

    void Awake()
    {
        beatBarDropdown.ClearOptions();
        beatBarDropdown.AddOptions(new List<string> { "Acima do jogador", "Fixa na tela" });
        BuildResolutions();

        beatBarDropdown.onValueChanged.AddListener(OnBeatBarChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        latencySlider.onValueChanged.AddListener(OnLatencyChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    void BuildResolutions()
    {
        _resolutions.Clear();
        var labels = new List<string>();

        foreach (var r in Screen.resolutions)
        {
            if (_resolutions.Exists(x => x.width == r.width && x.height == r.height)) continue;
            _resolutions.Add(r);
            labels.Add(r.width + " x " + r.height);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(labels);
    }

    void Refresh()
    {
        beatBarDropdown.SetValueWithoutNotify((int)GameSettings.BeatBar);
        volumeSlider.SetValueWithoutNotify(GameSettings.MasterVolume);
        latencySlider.SetValueWithoutNotify(GameSettings.AudioOffsetMs);
        UpdateLatencyLabel();

        int current = _resolutions.FindIndex(r => r.width == Screen.width && r.height == Screen.height);
        resolutionDropdown.SetValueWithoutNotify(Mathf.Max(0, current));
    }

    void OnBeatBarChanged(int value)
    {
        GameSettings.BeatBar = (BeatBarMode)value;
    }

    void OnVolumeChanged(float value)
    {
        GameSettings.MasterVolume = value;
    }

    void OnLatencyChanged(float value)
    {
        GameSettings.AudioOffsetMs = Mathf.RoundToInt(value);
        UpdateLatencyLabel();
    }

    void OnResolutionChanged(int index)
    {
        var r = _resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
    }

    void UpdateLatencyLabel()
    {
        latencyLabel.text = $"Latência: {GameSettings.AudioOffsetMs} ms";
    }
}
