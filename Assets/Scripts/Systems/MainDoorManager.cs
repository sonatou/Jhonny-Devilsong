using UnityEngine;
using UnityEngine.SceneManagement;

public class MainDoorManager : MonoBehaviour
{
    public static MainDoorManager Instance { get; private set; }

    [SerializeField] private int totalDoors = 4;

    private int _doorsRemaining;

    void Awake()
    {
        Instance = this;
        _doorsRemaining = totalDoors;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void NotifyDoorDestroyed()
    {
        _doorsRemaining--;

        if (_doorsRemaining <= 0)
        {
            ScoreManager.Instance?.SaveRun();
            SceneManager.LoadScene("Win");
        }
    }
}