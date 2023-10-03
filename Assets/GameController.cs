using UnityEngine;

public class GameController : MonoBehaviour
{
    private int minZoom = 1;
    private int maxZoom = 10;
    private float zoomStep = 3f;
    private float timeScale;
    public Grid grid;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Time.timeScale /= 2;
            if (Time.timeScale < 1f / 16) Time.timeScale = 0;
        }
            
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Time.timeScale = Mathf.Clamp(Time.timeScale * 2, 1f/16, 16f);

        if (Input.GetKey(KeyCode.UpArrow))
            Camera.main.orthographicSize = Mathf.Max(minZoom, Camera.main.orthographicSize - zoomStep * Time.unscaledDeltaTime);

        if (Input.GetKey(KeyCode.DownArrow))
            Camera.main.orthographicSize = Mathf.Min(maxZoom, Camera.main.orthographicSize + zoomStep * Time.unscaledDeltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0)
                Time.timeScale = timeScale;
            else
            {
                timeScale = Time.timeScale;
                Time.timeScale = 0;
            }
        }
    }
}
