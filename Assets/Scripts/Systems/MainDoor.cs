using UnityEngine;

public class MainDoor : MonoBehaviour
{
    [SerializeField] public string roomName;

    void Start()
    {
        TrySpawnPlayer();

        if (GameManager.Instance != null && GameManager.Instance.IsRoomCompleted(roomName))
            Invoke(nameof(DestroySelf), 0.1f);
    }

    void TrySpawnPlayer()
    {
        if (GameManager.Instance?.LastRoom != roomName) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = transform.position;
    }

    void DestroySelf()
    {
        MainDoorManager.Instance?.NotifyDoorDestroyed();
        Destroy(gameObject);
    }
}