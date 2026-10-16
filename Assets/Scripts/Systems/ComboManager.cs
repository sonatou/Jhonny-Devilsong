using UnityEngine;
using System;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    [Header("Combo")]
    [SerializeField] private int maxCombo = 4;

    public event Action<int> OnComboChanged;

    public int CurrentCombo { get; private set; }
    public int PerfectCount { get; private set; }
    public int Multiplier => Mathf.Max(1, CurrentCombo);

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
        if (quality == HitQuality.Perfect)
        {
            PerfectCount++;
            CurrentCombo = Mathf.Clamp(CurrentCombo + 1, 1, maxCombo);
        }
        else
        {
            CurrentCombo = 0;
        }

        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void ResetRun()
    {
        CurrentCombo = 0;
        PerfectCount = 0;
        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void ResetCombo()
    {
        CurrentCombo = 0;
        OnComboChanged?.Invoke(CurrentCombo);
    }
}
