using System;
using System.Threading;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum HitQuality { Perfect, Good, Miss }

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [Header("Evento FMOD")]
    [SerializeField] private EventReference musicEvent;

    [Header("Janela de Acerto")]
    [SerializeField, Range(0f, 0.3f)] private float perfectWindow = 0.05f;
    [SerializeField, Range(0f, 0.3f)] private float goodWindow = 0.15f;

    [Header("Feedback de Miss")]
    [SerializeField] private float missVolumeMultiplier = 0.4f;
    [SerializeField] private float missVolumeDuration = 0.3f;

    private static readonly System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
    private static int _pendingBeats;
    private static long _lastBeatTicks;

    private EventInstance _musicInstance;
    private EVENT_CALLBACK _beatCallback;
    private float _missTimer;
    private float _bpm = 130f;
    private double _nextBeatTime;
    private double _lastBeatTime;
    private bool _isPlaying;

    public event Action OnBeat;

    public static double Clock => _stopwatch.Elapsed.TotalSeconds;

    public float BeatInterval => 60f / _bpm;
    public double LastBeatTime => _lastBeatTime;
    public bool IsInWindow => GetAccuracy() <= goodWindow;
    public float GoodWindow => goodWindow;
    public float PerfectWindow => perfectWindow;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _beatCallback = BeatEventCallback;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleAudioForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleAudioForScene(scene.name);
    }

    public void OnMiss()
    {
        if (!_isPlaying) return;

        _missTimer = missVolumeDuration;
        _musicInstance.setParameterByName("solo_volume", missVolumeMultiplier);
    }

    void HandleAudioForScene(string sceneName)
    {
        bool isRun = sceneName != "MainMenu" &&
                     sceneName != "Hub" &&
                     sceneName != "GameOver" &&
                     sceneName != "Win";

        if (isRun)
        {
            if (!_isPlaying)
                PlayMusic();
        }
        else
        {
            StopMusic();
        }
    }

    void PlayMusic()
    {
        _musicInstance = RuntimeManager.CreateInstance(musicEvent);
        _musicInstance.setCallback(_beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        _musicInstance.setParameterByName("solo_volume", 1f);
        Interlocked.Exchange(ref _pendingBeats, 0);
        _musicInstance.start();
        _isPlaying = true;
    }

    void StopMusic()
    {
        if (!_isPlaying) return;

        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicInstance.release();
        _isPlaying = false;
    }

    void Update()
    {
        if (!_isPlaying) return;

        if (_missTimer > 0f)
        {
            _missTimer -= Time.deltaTime;
            if (_missTimer <= 0f)
                _musicInstance.setParameterByName("solo_volume", 1f);
        }
    }

    void LateUpdate()
    {
        if (!_isPlaying)
        {
            Interlocked.Exchange(ref _pendingBeats, 0);
            return;
        }

        if (Interlocked.Exchange(ref _pendingBeats, 0) == 0) return;

        _lastBeatTime = Interlocked.Read(ref _lastBeatTicks) / (double)System.Diagnostics.Stopwatch.Frequency;
        _nextBeatTime = _lastBeatTime + BeatInterval;
        OnBeat?.Invoke();
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            var parameter = (TIMELINE_BEAT_PROPERTIES)System.Runtime.InteropServices.Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));

            if (Instance != null)
            {
                Instance._bpm = parameter.tempo;
                Interlocked.Exchange(ref _lastBeatTicks, _stopwatch.ElapsedTicks);
                Interlocked.Increment(ref _pendingBeats);
            }
        }

        return FMOD.RESULT.OK;
    }

    public float GetAccuracy()
    {
        double currentTime = Clock;

        double distToLast = Math.Abs(currentTime - _lastBeatTime);
        double distToNext = Math.Abs(currentTime - _nextBeatTime);

        return (float)Math.Min(distToLast, distToNext);
    }

    public HitQuality GetHitQuality()
    {
        float accuracy = GetAccuracy();

        if (accuracy <= perfectWindow)
            return HitQuality.Perfect;

        if (accuracy <= goodWindow)
            return HitQuality.Good;

        return HitQuality.Miss;
    }
}
