using System.Collections;
using TMPro;
using UnityEngine;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text price;
    [SerializeField] private Vector2 offset = new Vector2(0f, 120f);
    [SerializeField] private float growDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.1f;

    private Coroutine _tween;

    public void Show(SkillData data, string priceText, Vector3 anchor)
    {
        title.text = data.DisplayName;
        description.text = data.Description;
        price.text = priceText;
        transform.position = anchor + (Vector3)(offset * transform.parent.lossyScale.x);

        bool wasVisible = gameObject.activeSelf;
        gameObject.SetActive(true);
        if (_tween != null) StopCoroutine(_tween);
        if (!wasVisible) transform.localScale = Vector3.zero;
        _tween = StartCoroutine(AnimateScale(true));
    }

    public void Hide()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_tween != null) StopCoroutine(_tween);
        _tween = StartCoroutine(AnimateScale(false));
    }

    IEnumerator AnimateScale(bool visible)
    {
        Vector3 start = transform.localScale;
        Vector3 target = visible ? Vector3.one : Vector3.zero;
        float duration = visible ? growDuration : shrinkDuration;

        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float eased = 1f - Mathf.Pow(1f - elapsed / duration, 3f);
            transform.localScale = Vector3.LerpUnclamped(start, target, eased);
            yield return null;
        }

        transform.localScale = target;
        _tween = null;
        if (!visible) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (_tween != null) StopCoroutine(_tween);
        _tween = null;
        transform.localScale = Vector3.one;
    }
}
