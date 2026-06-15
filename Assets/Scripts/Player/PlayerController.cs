using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;
    [SerializeField] private GameObject spriteObject;
    private Vector2 _input;
    private InputSystem_Actions _actions;
    private Camera _cam;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        DontDestroyOnLoad(gameObject);
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

        if (_input.x < 0)
        {
            spriteObject.gameObject.transform.localScale = new Vector3(-1,1,1);
        }
        else if (_input.x > 0)
        {
            spriteObject.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }


    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _input * moveSpeed;
    }
}