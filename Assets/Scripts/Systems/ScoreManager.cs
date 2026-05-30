using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Pontuacao")]
    [SerializeField] private int perfectPoints = 300;
    [SerializeField] private int goodPoints    = 100;
    [SerializeField] private int missPoints    = 10;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;

    public int CurrentScore    { get; private set; }
    public int PerfectCount    { get; private set; }
    public int CurrentCombo    { get; private set; }
    public int LastRunScore    { get; private set; }
    public int LastRunPerfects { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterHit(HitQuality quality)
    {
        int basePoints = quality switch
        {
            HitQuality.Perfect => perfectPoints,
            HitQuality.Good    => goodPoints,
            HitQuality.Miss    => missPoints,
            _                  => 0
        };

        if (quality == HitQuality.Perfect)
        {
            PerfectCount++;
            CurrentCombo = Mathf.Clamp(CurrentCombo + 1, 1, 4);
        }
        else
        {
            CurrentCombo = 0;
        }

        int multiplier   = Mathf.Max(1, CurrentCombo);
        int pointsEarned = basePoints * multiplier;

        CurrentScore += pointsEarned;

        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void SaveRun()
    {
        LastRunScore    = CurrentScore;
        LastRunPerfects = PerfectCount;
    }

    public void ResetRun()
    {
        CurrentScore = 0;
        PerfectCount = 0;
        CurrentCombo = 0;
        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void ResetCombo()
    {
        CurrentCombo = 0;
        OnComboChanged?.Invoke(CurrentCombo);
    }
}
