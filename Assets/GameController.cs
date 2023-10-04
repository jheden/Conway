using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private int minZoom = 1;
    private int maxZoom = 10;
    private float zoomStep = 3f;
    private float timeScale = 1f;
    public Grid grid;

    private InputActionAsset actions;
    public float ZoomInput { get; private set; }

    private void Awake()
    {
        actions = GetComponent<PlayerInput>().actions;
        actions["Pause"].started += OnPause;
        actions["SpeedDown"].started += OnSpeedDown;
        actions["SpeedUp"].started += OnSpeedUp;
        actions["Zoom"].started += ctx => ZoomInput = ctx.ReadValue<float>() * -zoomStep;
        actions["Zoom"].canceled += ctx => ZoomInput = 0f;
        actions.Enable();

        Time.timeScale = 0;
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0)
            Time.timeScale = timeScale;
        else
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    private void OnSpeedDown(InputAction.CallbackContext ctx)
    {
        Time.timeScale /= 2;
        if (Time.timeScale < 1f / 16) Time.timeScale = 0;
    }

    private void OnSpeedUp(InputAction.CallbackContext ctx)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale * 2, 1f / 16, 16f);
    }

    private void Update()
    {
        if (ZoomInput != 0f)
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + ZoomInput * Time.unscaledDeltaTime, minZoom, maxZoom);
    }
}
