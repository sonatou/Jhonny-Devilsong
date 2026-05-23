using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameObject managersPrefab;

    void Awake()
    {
        if (BeatManager.Instance != null) return;

        GameObject managers = Instantiate(managersPrefab);
        DontDestroyOnLoad(managers);
    }
}