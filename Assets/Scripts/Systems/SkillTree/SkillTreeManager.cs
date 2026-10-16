using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance { get; private set; }

    private const string UnlockedKey = "UnlockedSkills";

    [SerializeField] private SkillData[] skills;

    private readonly HashSet<SkillData> _unlocked = new HashSet<SkillData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Load();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool IsUnlocked(SkillData skill)
    {
        return _unlocked.Contains(skill);
    }

    public bool RequirementsMet(SkillData skill)
    {
        foreach (var requirement in skill.Requirements)
            if (requirement != null && !IsUnlocked(requirement))
                return false;

        return true;
    }

    public bool CanUnlock(SkillData skill)
    {
        return skill != null && !IsUnlocked(skill) && RequirementsMet(skill) && Wallet.Coins >= skill.Cost;
    }

    public bool TryUnlock(SkillData skill)
    {
        if (!CanUnlock(skill) || !Wallet.TrySpend(skill.Cost)) return false;

        _unlocked.Add(skill);
        Save();
        PlayerStats.Recalculate(_unlocked);
        return true;
    }

    void Load()
    {
        _unlocked.Clear();
        string[] saved = PlayerPrefs.GetString(UnlockedKey, string.Empty).Split(';');

        foreach (var skill in skills)
            if (skill != null && Array.IndexOf(saved, skill.name) >= 0)
                _unlocked.Add(skill);

        PlayerStats.Recalculate(_unlocked);
    }

    void Save()
    {
        var names = new List<string>();
        foreach (var skill in _unlocked)
            names.Add(skill.name);

        PlayerPrefs.SetString(UnlockedKey, string.Join(";", names));
        PlayerPrefs.Save();
    }

    [ContextMenu("Resetar progresso")]
    void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedKey);
        Wallet.Clear();
        Load();
    }
}
