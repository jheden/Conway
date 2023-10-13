using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    private float timeScale = 1f;
    public ConwayGrid grid;
    public Texture2D introText;
    public Texture2D introText2;

    private void Start()
    {
        Time.timeScale = 0;
        Cursor.visible = false;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private void DisplayIntro()
    {
        grid.DrawShape(grid.Resolution.x / 2, grid.Resolution.y / 4, new Shape(introText));
        grid.DrawShape(grid.Resolution.x / 2, grid.Resolution.y / 4 * 3, new Shape(introText2));
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
}
