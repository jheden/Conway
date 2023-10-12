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

    //public UnityEvent<Rect> resize;
    public delegate void Resize(Rect rect);
    public Resize resize;

    private void Update()
    {
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        var orthoGraphicSize = new Vector2(
            Camera.main.orthographicSize * Screen.width / Screen.height,
            Camera.main.orthographicSize
        );

        var bounds = new Rect(-orthoGraphicSize, orthoGraphicSize * 2);

        if (bounds == Bounds) return;

        Bounds = bounds;

        resize?.Invoke(Bounds);
    }
}