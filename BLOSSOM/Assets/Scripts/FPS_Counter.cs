using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
    public Text fpsDisplay;

    float frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    
    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f/updateRate)
        {
            fps = frameCount / dt ;
            frameCount = 0;
            dt -= 1.0f/updateRate;
            DisplayFPS();
        }
    }

    void DisplayFPS()
    {
        int ifps = (int)fps;
        fpsDisplay.text = ifps.ToString();
    }
}
