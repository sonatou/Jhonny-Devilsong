using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 3f;
    [SerializeField] private int fragments = 3;

    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float Knockback => knockback;
    public int Fragments => fragments;
}
