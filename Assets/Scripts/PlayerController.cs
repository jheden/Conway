using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActionAsset actions;
    public Vector2 AimInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool UseInput { get; private set; }
    
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        actions = GetComponent<PlayerInput>().actions;
        actions["Aim"].performed += ctx => AimInput = ctx.ReadValue<Vector2>();
        actions["Aim"].canceled += ctx => AimInput = Vector2.zero;
        actions["Use"].started += ctx => UseInput = true;
        actions["Use"].canceled += ctx => UseInput = false;
        actions.Enable();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
    }
}
