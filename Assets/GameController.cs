using UnityEngine;

public class GameController : MonoBehaviour
{
    private int minFrameRate = 10;
    private int maxFrameRate = 200;
    private int frameRateStep = 10;
    private int minZoom = 1;
    private int maxZoom = 40;
    private float zoomStep = 0.1f;
    public Grid grid;

    private void Start()
    {
        Application.targetFrameRate = 50;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Application.targetFrameRate = Mathf.Max(minFrameRate, Application.targetFrameRate - frameRateStep);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Application.targetFrameRate = Mathf.Min(maxFrameRate, Application.targetFrameRate + frameRateStep);

        if (Input.GetKey(KeyCode.UpArrow))
            Camera.main.orthographicSize = Mathf.Max(minZoom, Camera.main.orthographicSize - zoomStep);

        if (Input.GetKey(KeyCode.DownArrow))
            Camera.main.orthographicSize = Mathf.Min(maxZoom, Camera.main.orthographicSize + zoomStep);
    }
}
