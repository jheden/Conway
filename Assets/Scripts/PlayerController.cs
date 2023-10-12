using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour
{
    private InputActionAsset actions;
    public Vector2 MousePos {
        get => Mouse.current.position.ReadValue();
        set => InputState.Change(Mouse.current.position, value);
    }
    public Vector2 AimInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool UseInput { get; private set; }

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Vector2 _warpPos;

    void Awake()
    {
        actions = GetComponent<PlayerInput>().actions;
        actions["Aim"].performed += ctx => AimInput = ctx.ReadValue<Vector2>();
        actions["Aim"].canceled += ctx => AimInput = Vector2.zero;
        actions["Use"].started += ctx => UseInput = true;
        actions["Use"].canceled += ctx => UseInput = false;
        actions["Pause"].started += ctx => GameController.Instance.Pause();
        actions["SpeedDown"].started += ctx => GameController.Instance.SpeedDown();
        actions["SpeedUp"].started += ctx => GameController.Instance.SpeedUp();
        actions["Zoom"].started += ctx => GameController.Instance.ZoomInput = ctx.ReadValue<float>();
        actions["Zoom"].canceled += ctx => GameController.Instance.ZoomInput = 0f;
        actions["Rewind"].started += ctx => GameController.Instance.grid.Rewind = true;
        actions["Rewind"].canceled += ctx => GameController.Instance.grid.Rewind = false;
        actions.Enable();

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (AimInput != Vector2.zero)
        {
            _warpPos = MousePos + Time.unscaledDeltaTime * AimInput * 1000f;
            MousePos = new Vector2(Mathf.Clamp(_warpPos.x, 0, Screen.width), Mathf.Clamp(_warpPos.y, 0, Screen.height));
        }

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(MousePos);

        if (UseInput && Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit))
        {
            MeshCollider mesh = hit.collider as MeshCollider;
            if (mesh == null) return;
            if (mesh.TryGetComponent<ConwayGrid>(out ConwayGrid grid))
                grid.Click(hit.triangleIndex / 2);
            else if (mesh.TryGetComponent<ShapeSelector>(out ShapeSelector shapeSelector))
                shapeSelector.Click(hit.triangleIndex / 2);
        }
    }
}
