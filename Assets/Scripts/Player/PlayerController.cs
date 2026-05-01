using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Vector2 _input;
    private InputSystem_Actions _actions;
    private Camera _cam;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        _actions = new InputSystem_Actions();
        _cam = Camera.main;
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Move.performed += OnMove;
        _actions.Player.Move.canceled += OnMove;
    }

    void OnDisable()
    {
        _actions.Player.Move.performed -= OnMove;
        _actions.Player.Move.canceled -= OnMove;
        _actions.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _input = ctx.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _input * moveSpeed;
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mouseWorld - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}