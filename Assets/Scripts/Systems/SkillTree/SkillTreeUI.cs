using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI Instance { get; private set; }
    public static bool IsOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        IsOpen = false;
    }

    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private RectTransform linksRoot;
    [SerializeField] private SkillTooltip tooltip;

    [Header("Som")]
    [SerializeField] private EventReference buySound;
    [SerializeField] private EventReference deniedSound;

    [Header("Notas")]
    [SerializeField] private Sprite ownedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private float linkThickness = 4f;
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("Cores de upgrade")]
    [SerializeField] private Color upgradeLocked = new Color(0.35f, 0.08f, 0.10f);
    [SerializeField] private Color upgradeAvailable = new Color(0.25f, 0.80f, 0.35f);
    [SerializeField] private Color upgradeOwned = new Color(1f, 0.82f, 0.10f);

    [Header("Cores de habilidade")]
    [SerializeField] private Color abilityLocked = new Color(0.20f, 0.12f, 0.38f);
    [SerializeField] private Color abilityAvailable = new Color(0.35f, 0.65f, 1f);
    [SerializeField] private Color abilityOwned = new Color(0.78f, 0.45f, 1f);

    private readonly List<SkillNode> _nodes = new List<SkillNode>();
    private readonly List<Image> _links = new List<Image>();
    private readonly List<SkillData> _linkSources = new List<SkillData>();

    void Awake()
    {
        Instance = this;
        panel.GetComponentsInChildren(true, _nodes);
        foreach (var node in _nodes)
            node.Init(this);

        BuildLinks();
        panel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (!IsOpen) return;

        IsOpen = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!IsOpen) return;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.unscaledDeltaTime / fadeDuration);
        balanceText.text = $"$ {Wallet.Coins:N0}";

        float pulse = BeatManager.Instance != null ? pulseCurve.Evaluate(BeatManager.Instance.BeatPhase) : 0f;

        foreach (var node in _nodes)
        {
            bool owned = IsOwned(node.Skill);
            bool available = CanBuy(node.Skill);
            float scale = available ? 1f + pulse * pulseAmount : 1f;
            node.Refresh(StateColor(node.Skill.Type, owned, available), owned ? ownedSprite : openSprite, scale, !owned && !available);
        }

        for (int i = 0; i < _links.Count; i++)
        {
            bool owned = IsOwned(_linkSources[i]);
            Color color = StateColor(_linkSources[i].Type, owned, false);
            color.a = owned ? 1f : 0.5f;
            _links[i].color = color;
        }
    }

    public void Open()
    {
        if (IsOpen) return;

        IsOpen = true;
        Time.timeScale = 0f;
        canvasGroup.alpha = 0f;
        panel.SetActive(true);
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        Time.timeScale = 1f;
        tooltip.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    public void Buy(SkillNode node)
    {
        if (SkillTreeManager.Instance == null || !SkillTreeManager.Instance.TryUnlock(node.Skill))
        {
            PlaySound(deniedSound);
            return;
        }

        PlaySound(buySound);
        node.Pop();
        ShowTooltip(node);
    }

    public void ShowTooltip(SkillNode node)
    {
        tooltip.Show(node.Skill, PriceText(node.Skill), node.transform.position);
    }

    public void HideTooltip()
    {
        tooltip.Hide();
    }

    void PlaySound(EventReference sound)
    {
        if (!sound.IsNull)
            RuntimeManager.PlayOneShot(sound);
    }

    string PriceText(SkillData skill)
    {
        if (IsOwned(skill)) return "Comprado";
        if (SkillTreeManager.Instance != null && !SkillTreeManager.Instance.RequirementsMet(skill)) return "Bloqueado";
        return $"$ {skill.Cost:N0}";
    }

    bool IsOwned(SkillData skill)
    {
        return SkillTreeManager.Instance != null && SkillTreeManager.Instance.IsUnlocked(skill);
    }

    bool CanBuy(SkillData skill)
    {
        return SkillTreeManager.Instance != null && SkillTreeManager.Instance.CanUnlock(skill);
    }

    Color StateColor(SkillType type, bool owned, bool available)
    {
        bool ability = type == SkillType.Ability;
        if (owned) return ability ? abilityOwned : upgradeOwned;
        if (available) return ability ? abilityAvailable : upgradeAvailable;
        return ability ? abilityLocked : upgradeLocked;
    }

    void BuildLinks()
    {
        foreach (var node in _nodes)
        {
            foreach (var requirement in node.Skill.Requirements)
            {
                SkillNode from = _nodes.Find(n => n.Skill == requirement);
                if (from == null) continue;

                _links.Add(CreateLink(from.transform.localPosition, node.transform.localPosition));
                _linkSources.Add(requirement);
            }
        }
    }

    Image CreateLink(Vector2 from, Vector2 to)
    {
        var link = new GameObject("Link", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        link.raycastTarget = false;

        RectTransform rect = link.rectTransform;
        rect.SetParent(linksRoot, false);

        Vector2 direction = to - from;
        rect.localPosition = (from + to) / 2f;
        rect.sizeDelta = new Vector2(direction.magnitude, linkThickness);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        return link;
    }
}
