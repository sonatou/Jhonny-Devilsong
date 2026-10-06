using System;
using System.Runtime.InteropServices;
using FMODUnity;
using UnityEngine;

public class AudioSpectrum : MonoBehaviour
{
    public static AudioSpectrum Instance { get; private set; }

    [Header("FFT")]
    [SerializeField] private int windowSize = 1024;

    [Header("Bandas (Hz)")]
    [SerializeField] private Vector2 bassRange = new Vector2(20f, 250f);
    [SerializeField] private Vector2 midRange = new Vector2(250f, 2000f);
    [SerializeField] private Vector2 highRange = new Vector2(2000f, 8000f);

    [Header("Suavizacao")]
    [SerializeField] private float riseSpeed = 20f;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float peakDecay = 0.3f;
    [SerializeField] private float minPeak = 0.001f;

    public float Bass { get; private set; }
    public float Mid { get; private set; }
    public float High { get; private set; }

    private FMOD.DSP _fft;
    private FMOD.ChannelGroup _master;
    private float[] _spectrum;
    private float _binHz;
    private bool _attached;
    private float _bassPeak;
    private float _midPeak;
    private float _highPeak;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        var core = RuntimeManager.CoreSystem;

        if (core.createDSPByType(FMOD.DSP_TYPE.FFT, out _fft) != FMOD.RESULT.OK) return;

        _fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, windowSize);
        _fft.setParameterInt((int)FMOD.DSP_FFT.DOWNMIX, (int)FMOD.DSP_FFT_DOWNMIX_TYPE.MONO);

        core.getMasterChannelGroup(out _master);
        _master.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fft);

        core.getSoftwareFormat(out int sampleRate, out _, out _);
        _binHz = (float)sampleRate / windowSize;
        _spectrum = new float[windowSize / 2];
        _attached = true;
    }

    void OnDestroy()
    {
        if (!_attached) return;

        _master.removeDSP(_fft);
        _fft.release();
    }

    void Update()
    {
        if (!_attached) return;

        _fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out IntPtr data, out uint length);
        var fft = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(FMOD.DSP_PARAMETER_FFT));
        if (fft.numchannels == 0 || fft.length == 0) return;

        fft.getSpectrum(0, ref _spectrum);

        Bass = Smooth(Bass, Normalize(BandAverage(bassRange), ref _bassPeak));
        Mid = Smooth(Mid, Normalize(BandAverage(midRange), ref _midPeak));
        High = Smooth(High, Normalize(BandAverage(highRange), ref _highPeak));
    }

    float BandAverage(Vector2 range)
    {
        int start = Mathf.Clamp(Mathf.FloorToInt(range.x / _binHz), 0, _spectrum.Length - 1);
        int end = Mathf.Clamp(Mathf.CeilToInt(range.y / _binHz), start + 1, _spectrum.Length);

        float sum = 0f;
        for (int i = start; i < end; i++)
            sum += _spectrum[i];

        return sum / (end - start);
    }

    float Normalize(float value, ref float peak)
    {
        peak = Mathf.Max(value, peak * (1f - peakDecay * Time.deltaTime), minPeak);
        return value / peak;
    }

    float Smooth(float current, float target)
    {
        float speed = target > current ? riseSpeed : fallSpeed;
        return Mathf.MoveTowards(current, target, speed * Time.deltaTime);
    }
}
