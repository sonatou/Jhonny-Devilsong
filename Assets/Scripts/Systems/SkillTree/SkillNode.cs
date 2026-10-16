using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private SkillData skill;

    [Header("Visual")]
    [SerializeField] private RectTransform visual;
    [SerializeField] private Image head;
    [SerializeField] private Image stem;
    [SerializeField] private TMP_Text label;
    [SerializeField] private float lockedLabelAlpha = 0.45f;

    [Header("Compra")]
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popDuration = 0.35f;

    private SkillTreeUI _tree;
    private float _popTimer;

    public SkillData Skill => skill;

    public void Init(SkillTreeUI tree)
    {
        _tree = tree;
        label.text = skill.DisplayName;
    }

    public void Refresh(Color color, Sprite headSprite, float scale, bool locked)
    {
        head.sprite = headSprite;
        head.color = color;
        stem.color = color;
        label.alpha = locked ? lockedLabelAlpha : 1f;

        _popTimer = Mathf.Max(0f, _popTimer - Time.unscaledDeltaTime);
        float pop = popDuration > 0f ? _popTimer / popDuration : 0f;
        visual.localScale = Vector3.one * scale * Mathf.Lerp(1f, popScale, pop);
    }

    public void Pop()
    {
        _popTimer = popDuration;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tree.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tree.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tree.Buy(this);
    }
}
