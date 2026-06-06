using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRoomAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 anchorPosition;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SnapToAnchor();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SnapToAnchor();
    }

    void SnapToAnchor()
    {
        GameObject anchor = GameObject.FindWithTag("CameraAnchor");
        if (anchor != null)
            transform.position = new Vector3(
                anchor.transform.position.x,
                anchor.transform.position.y,
                transform.position.z);
        else
            transform.position = new Vector3(
                anchorPosition.x,
                anchorPosition.y,
                transform.position.z);
    }
}