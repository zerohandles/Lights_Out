using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string screenshotName;
            int randomNumber = Random.Range(0, 10000);

            screenshotName = "ScreenShot" + randomNumber + ".png";

            ScreenCapture.CaptureScreenshot(screenshotName);
        }
    }
}
