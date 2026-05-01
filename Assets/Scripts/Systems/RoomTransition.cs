using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string roomLabel = "Pressione E para entrar";
    [SerializeField] private bool locked = false;

    private bool _playerNearby = false;
    private PlayerHUD _hud;

    void Start()
    {
        _hud = FindAnyObjectByType<PlayerHUD>();
    }

    void Update()
    {
        if (_playerNearby && !locked && Input.GetKeyDown(KeyCode.E))
            SceneManager.LoadScene(targetScene);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;
        if (!locked) _hud?.ShowPrompt(roomLabel);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        _hud?.HidePrompt();
    }

    public void Lock()
    {
        locked = true;
        _hud?.HidePrompt();
    }

    public void Unlock()
    {
        locked = false;
        if (_playerNearby) _hud?.ShowPrompt(roomLabel);
    }
}