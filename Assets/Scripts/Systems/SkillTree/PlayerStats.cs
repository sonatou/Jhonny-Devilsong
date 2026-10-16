using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static int DamageBonus { get; private set; }
    public static float DamageMultiplier { get; private set; } = 1f;
    public static float MoneyMultiplier { get; private set; } = 1f;
    public static bool DashUnlocked { get; private set; }
    public static float DashDistanceBonus { get; private set; }
    public static float DashCooldownMultiplier { get; private set; } = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Clear();
    }

    public static void Recalculate(IEnumerable<SkillData> skills)
    {
        Clear();
        foreach (var skill in skills)
            Apply(skill);
    }

    public static int ApplyDamage(int baseDamage)
    {
        return Mathf.RoundToInt((baseDamage + DamageBonus) * DamageMultiplier);
    }

    static void Clear()
    {
        DamageBonus = 0;
        DamageMultiplier = 1f;
        MoneyMultiplier = 1f;
        DashUnlocked = false;
        DashDistanceBonus = 0f;
        DashCooldownMultiplier = 1f;
    }

    static void Apply(SkillData skill)
    {
        switch (skill.Effect)
        {
            case SkillEffect.AddDamage:
                DamageBonus += Mathf.RoundToInt(skill.Value);
                break;

            case SkillEffect.MultiplyDamage:
                DamageMultiplier *= 1f + skill.Value / 100f;
                break;

            case SkillEffect.AddMoney:
                MoneyMultiplier += skill.Value / 100f;
                break;

            case SkillEffect.UnlockDash:
                DashUnlocked = true;
                break;

            case SkillEffect.AddDashDistance:
                DashDistanceBonus += skill.Value;
                break;

            case SkillEffect.ReduceDashCooldown:
                DashCooldownMultiplier *= 1f - skill.Value / 100f;
                break;
        }
    }
}
