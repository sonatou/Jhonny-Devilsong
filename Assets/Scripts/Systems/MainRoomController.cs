using UnityEngine;
using UnityEngine.SceneManagement;

public class MainRoomController : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.AllRoomsCompleted)
        {
            ScoreManager.Instance?.SaveRun();
            SceneManager.LoadScene("Win");
        }
    }
}