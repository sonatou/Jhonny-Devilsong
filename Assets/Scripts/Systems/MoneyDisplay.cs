using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text comboText;

    private int _shownMoney = -1;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        BindCombo();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindCombo();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnbindCombo();
        BindCombo();
    }

    void BindCombo()
    {
        if (ComboManager.Instance == null) return;

        ComboManager.Instance.OnComboChanged += UpdateCombo;
        UpdateCombo(ComboManager.Instance.CurrentCombo);
    }

    void UnbindCombo()
    {
        if (ComboManager.Instance == null) return;

        ComboManager.Instance.OnComboChanged -= UpdateCombo;
    }

    void Update()
    {
        int money = Wallet.Coins;
        if (money == _shownMoney) return;

        _shownMoney = money;
        moneyText.text = $"$ {money:N0}";
    }

    void UpdateCombo(int combo)
    {
        if (comboText == null) return;

        comboText.gameObject.SetActive(combo > 1);
        comboText.text = $"x{combo}";
    }
}
