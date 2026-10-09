using UnityEngine;

public class BeatBarAnchor : MonoBehaviour
{
    [Header("Acima do jogador")]
    [SerializeField] private Vector3 playerOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Fixa na tela")]
    [SerializeField] private Vector2 viewportPosition = new Vector2(0.5f, 0.08f);
    [SerializeField] private float screenScale = 2f;

    private Transform _player;
    private BeatBarMode _mode;
    private Vector3 _baseScale;

    void Start()
    {
        _player = transform.parent;
        _baseScale = transform.localScale;
        ApplyMode();
        GameSettings.Changed += ApplyMode;
    }

    void OnDestroy()
    {
        GameSettings.Changed -= ApplyMode;
    }

    void ApplyMode()
    {
        _mode = GameSettings.BeatBar;
        transform.localScale = _mode == BeatBarMode.FixedScreen ? _baseScale * screenScale : _baseScale;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;

        if (_mode == BeatBarMode.AbovePlayer)
        {
            transform.position = _player.position + playerOffset;
            return;
        }

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 point = cam.ViewportToWorldPoint(viewportPosition);
        transform.position = new Vector3(point.x, point.y, _player.position.z);
    }
}
