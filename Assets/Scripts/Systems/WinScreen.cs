using UnityEngine;
using TMPro;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text perfectText;

    void Start()
    {
        int perfects = ComboManager.Instance != null ? ComboManager.Instance.PerfectCount : 0;

        moneyText.text = $"Dinheiro: $ {Wallet.RunEarnings:N0}";
        perfectText.text = $"Perfects: {perfects}";
    }

    public void ReturnToHub()
    {
        ComboManager.Instance?.ResetRun();
        Wallet.ResetRun();
        GameManager.Instance?.ResetRooms();
        SceneTransition.Load("Hub");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
