using UnityEngine;

public enum SkillType { Upgrade, Ability }

public enum SkillEffect
{
    AddDamage,
    MultiplyDamage,
    AddMoney,
    UnlockDash,
    AddDashDistance,
    ReduceDashCooldown
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill Tree/Skill")]
public class SkillData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private int cost = 1000;
    [SerializeField] private SkillType type;

    [Header("Efeito")]
    [SerializeField] private SkillEffect effect;
    [SerializeField] private float value;

    [Header("Requisitos")]
    [SerializeField] private SkillData[] requirements;

    public string DisplayName => displayName;
    public string Description => description;
    public int Cost => cost;
    public SkillType Type => type;
    public SkillEffect Effect => effect;
    public float Value => value;
    public SkillData[] Requirements => requirements;
}
