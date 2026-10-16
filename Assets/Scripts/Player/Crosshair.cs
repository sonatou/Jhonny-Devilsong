using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ring;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private string[] menuScenes = { "MainMenu", "GameOver", "Win" };

    [Header("Pulso no beat")]
    [SerializeField] private float pulseScale = 1.35f;
    [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private Vector3 _baseScale;
    private bool _inMenu;
    private bool _wasVisible;

    void Awake()
    {
        _baseScale = ring.transform.localScale;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _inMenu = IsMenu(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Cursor.visible = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _inMenu = IsMenu(scene.name);
        trail.Clear();
    }

    bool IsMenu(string sceneName)
    {
        return System.Array.IndexOf(menuScenes, sceneName) >= 0;
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        bool visible = !_inMenu && !PauseMenu.IsPaused && !SkillTreeUI.IsOpen && cam != null && Mouse.current != null;

        Cursor.visible = !visible;
        ring.enabled = visible;
        trail.emitting = visible;

        if (!visible)
        {
            _wasVisible = false;
            return;
        }

        Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = new Vector3(world.x, world.y, 0f);
        if (!_wasVisible) trail.Clear();
        _wasVisible = true;

        float pulse = BeatManager.Instance != null ? pulseCurve.Evaluate(BeatManager.Instance.BeatPhase) : 0f;
        ring.transform.localScale = _baseScale * Mathf.Lerp(1f, pulseScale, pulse);
    }
}
