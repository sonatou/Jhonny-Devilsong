using UnityEngine;

public class MainRoomController : MonoBehaviour
{
    [SerializeField] private SpawnPortal[] portals;

    void Start()
    {
        string lastRoom = GameManager.Instance?.LastRoom;
        if (string.IsNullOrEmpty(lastRoom)) return;

        foreach (var portal in portals)
        {
            if (portal.roomName == lastRoom)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                    player.transform.position = portal.transform.position;
                return;
            }
        }
    }
}