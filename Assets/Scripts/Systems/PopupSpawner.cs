using UnityEngine;

public class PopupSpawner : MonoBehaviour
{
    public static PopupSpawner Instance { get; private set; }

    [SerializeField] private TextPopup damagePopup;
    [SerializeField] private TextPopup moneyPopup;

    [Header("Cores")]
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private Color moneyColor = new Color(1f, 0.82f, 0.10f);

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ShowDamage(int amount, Vector3 worldPosition)
    {
        Spawn(damagePopup, amount.ToString(), damageColor, worldPosition);
    }

    public void ShowMoney(int amount, Vector3 worldPosition)
    {
        Spawn(moneyPopup, $"+$ {amount}", moneyColor, worldPosition);
    }

    void Spawn(TextPopup prefab, string text, Color color, Vector3 worldPosition)
    {
        Camera cam = Camera.main;
        if (prefab == null || cam == null) return;

        TextPopup popup = Instantiate(prefab, transform, false);
        popup.Initialize(text, color, worldPosition, cam);
    }
}
