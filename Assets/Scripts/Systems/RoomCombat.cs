using System.Collections.Generic;
using UnityEngine;

public class RoomCombat : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    private List<GameObject> _enemies = new List<GameObject>();
    private RoomTransition[] _doors;
    private bool _cleared = false;

    void Start()
    {
        var allDoors = FindObjectsByType<RoomTransition>(FindObjectsSortMode.None);
        var sceneDoors = new List<RoomTransition>();

        foreach (var door in allDoors)
        {
            if (door.gameObject.scene == gameObject.scene)
                sceneDoors.Add(door);
        }

        _doors = sceneDoors.ToArray();

        string currentScene = gameObject.scene.name;

        if (GameManager.Instance != null && GameManager.Instance.IsRoomCompleted(currentScene))
        {
            _cleared = true;
            return;
        }

        StartCombat();
    }

    void Update()
    {
        if (_cleared) return;

        _enemies.RemoveAll(e => e == null);

        if (_enemies.Count == 0)
            ClearRoom();
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

        GameManager.Instance?.CompleteRoom(gameObject.scene.name);
    }
}
