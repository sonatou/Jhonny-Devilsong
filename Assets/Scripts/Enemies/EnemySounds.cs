using FMODUnity;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] private EventReference hitSound;
    [SerializeField] private EventReference deathSound;

    void Awake()
    {
        EnemyBase enemy = GetComponent<EnemyBase>();
        enemy.OnHit += () => Play(hitSound);
        enemy.OnDeath += () => Play(deathSound);
    }

    void Play(EventReference sound)
    {
        if (!sound.IsNull)
            RuntimeManager.PlayOneShot(sound, transform.position);
    }
}
