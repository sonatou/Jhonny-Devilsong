using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCombat : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    [Header("Ondas")]
    [SerializeField] private int extraWaves = 1;
    [SerializeField] private int waveSize = 3;
    [SerializeField] private float waveDelay = 1f;

    private List<GameObject> _enemies = new List<GameObject>();
    private RoomTransition[] _doors;
    private bool _cleared = false;
    private int _wavesLeft;
    private bool _spawningWave;

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
        if (_cleared || _spawningWave) return;

        _enemies.RemoveAll(e => e == null);
        if (_enemies.Count > 0) return;

        if (_wavesLeft > 0)
            StartCoroutine(NextWave());
        else
            ClearRoom();
    }

    IEnumerator NextWave()
    {
        _spawningWave = true;
        _wavesLeft--;
        yield return new WaitForSeconds(waveDelay);
        _enemies.AddRange(spawner.SpawnWave(waveSize));
        _spawningWave = false;
    }

    public void Track(GameObject enemy)
    {
        _enemies.Add(enemy);
    }

    void StartCombat()
    {
        foreach (var door in _doors)
            door.Lock();

        _wavesLeft = extraWaves;
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
