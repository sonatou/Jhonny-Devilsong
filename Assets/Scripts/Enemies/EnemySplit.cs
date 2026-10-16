using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemySplit : MonoBehaviour
{
    [SerializeField] private EnemyBase childPrefab;
    [SerializeField] private int count = 2;
    [SerializeField] private float expelDistance = 1.5f;
    [SerializeField] private float expelDuration = 0.25f;

    void Awake()
    {
        GetComponent<EnemyBase>().OnDeath += Split;
    }

    void Split()
    {
        if (childPrefab == null) return;

        RoomCombat room = FindAnyObjectByType<RoomCombat>();
        float startAngle = Random.Range(0f, 360f);
        bool champion = TryGetComponent(out EnemyChampion parentChampion) && parentChampion.IsChampion;

        for (int i = 0; i < count; i++)
        {
            float angle = (startAngle + i * 360f / count) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            EnemyBase child = Instantiate(childPrefab, transform.position, Quaternion.identity);
            child.gameObject.AddComponent<EnemyExpel>().Init(direction * expelDistance, expelDuration);
            if (champion) child.gameObject.AddComponent<EnemyChampion>().MakeChampion();

            if (room != null) room.Track(child.gameObject);
        }
    }
}
