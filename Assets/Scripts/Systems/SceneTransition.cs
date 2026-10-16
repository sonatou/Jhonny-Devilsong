using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class SceneTransition : MonoBehaviour
{
    public static bool IsLoading { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        IsLoading = false;
    }

    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float holdDuration = 0.1f;

    private CanvasGroup _overlay;

    public static void Load(string sceneName, Action beforeLoad = null)
    {
        if (IsLoading) return;

        SceneTransition prefab = Resources.Load<SceneTransition>("SceneTransition");
        if (prefab == null)
        {
            beforeLoad?.Invoke();
            SceneManager.LoadScene(sceneName);
            return;
        }

        Instantiate(prefab).Play(sceneName, beforeLoad);
    }

    void Play(string sceneName, Action beforeLoad)
    {
        IsLoading = true;
        DontDestroyOnLoad(gameObject);
        _overlay = GetComponent<CanvasGroup>();
        _overlay.alpha = 0f;
        StartCoroutine(Transition(sceneName, beforeLoad));
    }

    IEnumerator Transition(string sceneName, Action beforeLoad)
    {
        yield return Fade(0f, 1f);
        beforeLoad?.Invoke();
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForSecondsRealtime(holdDuration);
        IsLoading = false;
        yield return Fade(1f, 0f);
        Destroy(gameObject);
    }

    IEnumerator Fade(float from, float to)
    {
        for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.unscaledDeltaTime)
        {
            _overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration));
            yield return null;
        }

        _overlay.alpha = to;
    }
}
