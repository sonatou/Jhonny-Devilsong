using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public TMP_Text promptText;

    void Awake() => HidePrompt();

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}