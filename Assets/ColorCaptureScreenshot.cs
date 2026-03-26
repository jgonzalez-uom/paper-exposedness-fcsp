using UnityEngine;

public class ColorCaptureScreenshot : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Picture();
        }
    }

    void Picture()
    {
        string clipName = System.DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');

        ScreenCapture.CaptureScreenshot(string.Format("{0}/{1}.{2}", Application.persistentDataPath, clipName, "png"));

    }
}
