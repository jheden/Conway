using UnityEngine;

public class GameController : MonoBehaviour
{
    private int frameRate = 0;
    private int maxFrameRate = 200;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            frameRate = (frameRate + 25) % maxFrameRate;
            Application.targetFrameRate = frameRate;
        }

        if (Input.GetMouseButtonDown(1))
        {
            frameRate = (frameRate - 25 + maxFrameRate) % maxFrameRate;
            Application.targetFrameRate = frameRate;
        }
    }
}
