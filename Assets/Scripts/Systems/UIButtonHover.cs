using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float enterDuration = 0.6f;
    [SerializeField] private float exitDuration = 0.15f;

    private Vector3 _originalScale;
    private Coroutine _tween;

    void Awake()
    {
        _originalScale = transform.localScale;
    }

    void OnDisable()
    {
        if (_tween != null) StopCoroutine(_tween);
        _tween = null;
        transform.localScale = _originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animate(_originalScale * hoverScale, enterDuration, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animate(_originalScale, exitDuration, false);
    }

    void Animate(Vector3 target, float duration, bool elastic)
    {
        if (!isActiveAndEnabled) return;
        if (_tween != null) StopCoroutine(_tween);
        _tween = StartCoroutine(TweenScale(target, duration, elastic));
    }

    IEnumerator TweenScale(Vector3 target, float duration, bool elastic)
    {
        Vector3 start = transform.localScale;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float t = elapsed / duration;
            float eased = elastic ? Easing.OutElastic(t) : Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.LerpUnclamped(start, target, eased);
            yield return null;
        }

        transform.localScale = target;
        _tween = null;
    }
}
