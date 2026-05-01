using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Pontuação")]
    [SerializeField] private int perfectPoints = 300;
    [SerializeField] private int goodPoints = 100;
    [SerializeField] private int missPoints = 10;

    public event Action<int> OnScoreChanged;

    public int CurrentScore { get; private set; }
    public int PerfectCount { get; private set; }
    public int LastRunScore { get; private set; }
    public int LastRunPerfects { get; private set; }

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

    public void AddScore(HitQuality quality)
    {
        switch (quality)
        {
            case HitQuality.Perfect:
                CurrentScore += perfectPoints;
                PerfectCount++;
                break;
            case HitQuality.Good:
                CurrentScore += goodPoints;
                break;
            case HitQuality.Miss:
                CurrentScore += missPoints;
                break;
        }

        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void SaveRun()
    {
        LastRunScore = CurrentScore;
        LastRunPerfects = PerfectCount;
    }

    public void ResetRun()
    {
        CurrentScore = 0;
        PerfectCount = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}