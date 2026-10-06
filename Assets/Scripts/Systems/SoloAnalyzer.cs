using System;
using System.Runtime.InteropServices;
using FMODUnity;
using UnityEngine;

public class SoloAnalyzer : MonoBehaviour
{
    public static SoloAnalyzer Instance { get; private set; }

    [Header("Track")]
    [SerializeField] private int trackIndex = 1;
    [SerializeField] private int windowSize = 1024;

    [Header("Energia")]
    [SerializeField] private float riseSpeed = 20f;
    [SerializeField] private float fallSpeed = 4f;
    [SerializeField] private float peakDecay = 0.3f;
    [SerializeField] private float minPeak = 0.001f;

    [Header("Notas")]
    [SerializeField] private float onsetThreshold = 1.6f;
    [SerializeField] private float onsetCooldown = 0.08f;
    [SerializeField] private float averageSpeed = 4f;

    public float Energy { get; private set; }
    public event Action OnNote;

    private FMOD.DSP _fft;
    private FMOD.ChannelGroup _soloGroup;
    private IntPtr _attachedInstance;
    private bool _attached;
    private float[] _spectrum;
    private float[] _previous;
    private float _peak;
    private float _averageFlux;
    private float _lastOnset;

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
        core.createDSPByType(FMOD.DSP_TYPE.FFT, out _fft);
        _fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, windowSize);
        _fft.setParameterInt((int)FMOD.DSP_FFT.DOWNMIX, (int)FMOD.DSP_FFT_DOWNMIX_TYPE.MONO);

        _spectrum = new float[windowSize / 2];
        _previous = new float[windowSize / 2];
    }

    void OnDestroy()
    {
        Detach();
        if (_fft.hasHandle()) _fft.release();
    }

    void Update()
    {
        TryAttach();

        if (!_attached)
        {
            Energy = Mathf.MoveTowards(Energy, 0f, fallSpeed * Time.deltaTime);
            return;
        }

        _fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out IntPtr data, out uint length);
        var fft = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(FMOD.DSP_PARAMETER_FFT));
        if (fft.numchannels == 0 || fft.length == 0) return;

        fft.getSpectrum(0, ref _spectrum);

        float energy = 0f;
        float flux = 0f;
        for (int i = 0; i < _spectrum.Length; i++)
        {
            energy += _spectrum[i];
            flux += Mathf.Max(0f, _spectrum[i] - _previous[i]);
            _previous[i] = _spectrum[i];
        }

        _peak = Mathf.Max(energy, _peak * (1f - peakDecay * Time.deltaTime), minPeak);
        float target = energy / _peak;
        Energy = Mathf.MoveTowards(Energy, target, (target > Energy ? riseSpeed : fallSpeed) * Time.deltaTime);

        if (flux > _averageFlux * onsetThreshold && flux > minPeak && Time.time - _lastOnset > onsetCooldown)
        {
            _lastOnset = Time.time;
            OnNote?.Invoke();
        }

        _averageFlux = Mathf.Lerp(_averageFlux, flux, averageSpeed * Time.deltaTime);
    }

    void TryAttach()
    {
        var manager = BeatManager.Instance;
        if (manager == null || !manager.IsPlaying)
        {
            Detach();
            return;
        }

        var instance = manager.MusicInstance;
        if (_attached && instance.handle == _attachedInstance) return;

        Detach();
        if (instance.getChannelGroup(out FMOD.ChannelGroup eventGroup) != FMOD.RESULT.OK) return;

        eventGroup.getNumGroups(out int count);
        if (count <= trackIndex) return;

        eventGroup.getGroup(trackIndex, out _soloGroup);
        _soloGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fft);
        Array.Clear(_previous, 0, _previous.Length);
        _attachedInstance = instance.handle;
        _attached = true;
    }

    void Detach()
    {
        if (!_attached) return;

        _soloGroup.removeDSP(_fft);
        _attached = false;
    }
}
