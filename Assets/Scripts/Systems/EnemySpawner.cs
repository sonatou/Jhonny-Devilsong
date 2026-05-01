using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    public List<GameObject> SpawnAll()
    {
        List<GameObject> spawned = new List<GameObject>();

        foreach (Transform point in spawnPoints)
        {
            if (point == null || enemyPrefabs.Length == 0) continue;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
            spawned.Add(enemy);
        }

        return spawned;
    }
}