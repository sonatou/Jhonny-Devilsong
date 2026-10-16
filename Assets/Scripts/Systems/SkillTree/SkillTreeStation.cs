using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeStation : MonoBehaviour
{
    [SerializeField] private string promptLabel = "Pressione E para abrir as habilidades";

    private bool _playerNearby;
    private bool _opened;
    private PlayerHUD _hud;

    void Start()
    {
        _hud = FindAnyObjectByType<PlayerHUD>();
    }

    void Update()
    {
        if (_opened && !SkillTreeUI.IsOpen)
        {
            _opened = false;
            if (_playerNearby) _hud?.ShowPrompt(promptLabel);
        }

        if (!_playerNearby || SkillTreeUI.IsOpen || PauseMenu.IsPaused || SkillTreeUI.Instance == null) return;
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;

        _opened = true;
        _hud?.HidePrompt();
        SkillTreeUI.Instance.Open();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;
        _hud?.ShowPrompt(promptLabel);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        _hud?.HidePrompt();
    }
}
