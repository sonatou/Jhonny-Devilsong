using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HUDFade : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.25f;

    private CanvasGroup _group;

    void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        bool visible = !SkillTreeUI.IsOpen && !PauseMenu.IsPaused;
        _group.alpha = Mathf.MoveTowards(_group.alpha, visible ? 1f : 0f, Time.unscaledDeltaTime / fadeDuration);
    }
}
