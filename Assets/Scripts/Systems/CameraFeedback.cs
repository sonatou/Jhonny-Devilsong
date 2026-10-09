using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFeedback : MonoBehaviour
{
    public static CameraFeedback Instance { get; private set; }

    [Header("Shake")]
    [SerializeField] private float maxOffset = 0.35f;
    [SerializeField] private float traumaDecay = 2.5f;
    [SerializeField] private float shakeFrequency = 25f;

    private float _trauma;
    private Vector3 _lastOffset;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _lastOffset = Vector3.zero;
    }

    public void Shake(float amount)
    {
        _trauma = Mathf.Clamp01(_trauma + amount);
    }

    void LateUpdate()
    {
        transform.position -= _lastOffset;

        float shake = _trauma * _trauma;
        float t = Time.time * shakeFrequency;
        _lastOffset = new Vector3(Mathf.PerlinNoise(t, 0f) * 2f - 1f, Mathf.PerlinNoise(0f, t) * 2f - 1f, 0f) * maxOffset * shake;
        transform.position += _lastOffset;

        _trauma = Mathf.Max(0f, _trauma - traumaDecay * Time.deltaTime);
    }
}
