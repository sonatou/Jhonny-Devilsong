using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private HashSet<string> _completedRooms = new HashSet<string>();
    private static readonly string[] _requiredRooms = { "Seek", "Flee", "Wander", "Pursuit", "Main" };

    public bool AllRoomsCompleted
    {
        get
        {
            foreach (var room in _requiredRooms)
                if (!_completedRooms.Contains(room)) return false;
            return true;
        }
    }

    public enum GameState { Hub, InRun, GameOver }

    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Hub")
            SetState(GameState.Hub);
        else if (scene.name == "GameOver")
            SetState(GameState.GameOver);
        else
            SetState(GameState.InRun);
    }

    void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
    public void CompleteRoom(string sceneName)
    {
        _completedRooms.Add(sceneName);
    }

    public void ResetRooms()
    {
        _completedRooms.Clear();
    }

    public bool IsRoomCompleted(string sceneName)
    {
        return _completedRooms.Contains(sceneName);
    }
}
