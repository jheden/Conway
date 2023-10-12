using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    public static CameraController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    public Rect Bounds { get; private set; }

    public delegate void Resize(Rect rect);
    public Resize resize;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        var orthoGraphicSize = new Vector2(
            _camera.orthographicSize * _camera.aspect,
            _camera.orthographicSize
        );

        var bounds = new Rect(-orthoGraphicSize, orthoGraphicSize * 2);

        if (bounds == Bounds) return;

        Bounds = bounds;
        print(bounds);

        resize?.Invoke(Bounds);
    }
}