using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum HitQuality { Perfect, Good, Miss }

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [Header("Configuracao")]
    [SerializeField] private float bpm = 120f;
    [SerializeField] private AudioSource audioSource;

    [Header("Janela de Acerto")]
    [SerializeField, Range(0f, 0.3f)] private float perfectWindow = 0.05f;
    [SerializeField, Range(0f, 0.3f)] private float goodWindow    = 0.15f;

    public event Action OnBeat;

    public float BeatInterval  => 60f / bpm;
    public float BeatProgress  { get; private set; }
    public bool  IsInWindow    => GetAccuracy() <= goodWindow;
    public float GoodWindow    => goodWindow;
    public float PerfectWindow => perfectWindow;

    private double _nextBeatTime;
    private double _lastBeatTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleAudioForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleAudioForScene(scene.name);
    }

    void HandleAudioForScene(string sceneName)
    {
        bool isRun = sceneName != "Hub" && sceneName != "GameOver";

        if (isRun)
        {
            if (!audioSource.isPlaying)
            {
                _nextBeatTime = AudioSettings.dspTime + 1f;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Pause();
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying) return;

        double currentTime = AudioSettings.dspTime;
        BeatProgress = (float)((currentTime - _lastBeatTime) / BeatInterval);

        if (currentTime >= _nextBeatTime)
        {
            _lastBeatTime = _nextBeatTime;
            _nextBeatTime += BeatInterval;
            OnBeat?.Invoke();
        }
    }

    public float GetAccuracy()
    {
        double currentTime = AudioSettings.dspTime;
        double distToLast  = Math.Abs(currentTime - _lastBeatTime);
        double distToNext  = Math.Abs(currentTime - _nextBeatTime);
        return (float)Math.Min(distToLast, distToNext);
    }

    public HitQuality GetHitQuality()
    {
        float accuracy = GetAccuracy();
        if (accuracy <= perfectWindow) return HitQuality.Perfect;
        if (accuracy <= goodWindow)   return HitQuality.Good;
        return HitQuality.Miss;
    }
}
