using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureTaker : MonoBehaviour
{
    public Vector3[] values;
    bool executing = false;
    public KeyCode key = KeyCode.R;

    // Update is called once per frame
    void Update()
    {
        if (!executing && Input.GetKeyDown(key))
        {
            StartCoroutine(TakePhotos());
        }
    }

    IEnumerator TakePhotos()
    {
        executing = true;
        string clipName = System.DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
        System.IO.Directory.CreateDirectory(string.Format("{0}/AnglePhotos_{1}/", Application.persistentDataPath, clipName));

        for (int n = 0; n < values.Length; n++)
        {
            yield return null;

            transform.localEulerAngles = values[n];

            ScreenCapture.CaptureScreenshot(string.Format("{0}/AnglePhotos_{1}/{1}_{2}.{3}", Application.persistentDataPath, clipName, n, "png"));

        }

        executing = false;
    }
}
