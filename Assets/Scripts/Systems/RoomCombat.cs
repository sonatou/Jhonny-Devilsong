using System.Collections.Generic;
using UnityEngine;

public class RoomCombat : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    private List<GameObject> _enemies = new List<GameObject>();
    private RoomTransition[] _doors;
    private bool _triggered = false;
    private bool _cleared = false;

void Start()
    {
        _doors = FindObjectsByType<RoomTransition>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (!_triggered || _cleared) return;

        _enemies.RemoveAll(e => e == null);

        if (_enemies.Count == 0)
            ClearRoom();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered || _cleared) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        StartCombat();
    }

    void StartCombat()
    {
        foreach (var door in _doors)
            door.Lock();

        _enemies = spawner.SpawnAll();
    }

    void ClearRoom()
    {
        _cleared = true;

        foreach (var door in _doors)
            door.Unlock();
    }
}