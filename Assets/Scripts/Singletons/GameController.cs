using UnityEngine;

public class GameController : MonoBehaviour
{
    private int minZoom = 1;
    private int maxZoom = 10;
    private float zoomStep = 3f;
    private float timeScale = 1f;
    public ConwayGrid grid;
    public Texture2D introText;
    public Texture2D introText2;

    public float ZoomInput { get; set; }
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Time.timeScale = 0;
        Cursor.visible = false;
    }

    private void Start()
    {
    }

    private void DisplayIntro()
    {
        grid.AddShape(grid.Resolution.x / 2, grid.Resolution.y / 4, new Shape(introText));
        grid.AddShape(grid.Resolution.x / 2, grid.Resolution.y / 4 * 3, new Shape(introText2));
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
            Time.timeScale = timeScale;
        else
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    public void SpeedDown()
    {
        Time.timeScale /= 2;
        if (Time.timeScale < 1f / 16) Time.timeScale = 0;
    }

    public void SpeedUp()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale * 2, 1f / 16, 16f);
    }

    private void Update()
    {
        if (ZoomInput != 0f)
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + ZoomInput * -zoomStep * Time.unscaledDeltaTime, minZoom, maxZoom);
    }
}
