using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    [Tooltip("Nome exato da cena para onde esta porta leva.")]
    public string targetScene;

    [Tooltip("Texto exibido ao jogador quando ele se aproxima.")]
    public string roomLabel = "Pressione E para entrar";

    private bool playerNearby = false;
    private PlayerHUD hud;

    void Start()
    {
        hud = FindAnyObjectByType<PlayerHUD>();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            SceneManager.LoadScene(targetScene);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        hud?.ShowPrompt(roomLabel);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        hud?.HidePrompt();
    }
}