using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyChampion : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float chance = 0.15f;
    [SerializeField] private float healthMultiplier = 1.5f;
    [SerializeField] private float speedMultiplier = 1.2f;
    [SerializeField] private int lootMultiplier = 2;
    [SerializeField] private float sizeMultiplier = 1.15f;
    [SerializeField] private Color tint = new Color(1f, 0.4f, 0.4f);

    public bool IsChampion { get; private set; }

    void Awake()
    {
        if (Random.value < chance) MakeChampion();
    }

    public void MakeChampion()
    {
        if (IsChampion) return;
        IsChampion = true;

        EnemyBase enemy = GetComponent<EnemyBase>();
        enemy.HealthMultiplier = healthMultiplier;
        enemy.SpeedMultiplier = speedMultiplier;
        enemy.LootMultiplier = lootMultiplier;
        enemy.Body.color = tint;
        transform.localScale *= sizeMultiplier;
    }
}
