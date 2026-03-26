using UnityEngine;
using UnityEngine.UI;

public class SplitScreenshotScript : MonoBehaviour
{
    public int numRows = 5;
    public int numColumns = 5;
    public RenderTexture outputTexture;

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
        int width = Camera.main.pixelWidth / numColumns;
        int height = Camera.main.pixelHeight / numRows;
        int startX, startY;

        for (int x = 0; x < numColumns; x++)
        {
            for (int y = 0; y < numRows; y++)
            {
                startX = x * width;
                startY = y * height;

                var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

                Rect rex = new Rect(startX, startY, width, height);

                RenderTexture.active = outputTexture;
                Camera.main.Render();

                tex.ReadPixels(rex, 0, 0);
                tex.Apply();

                // Encode texture into PNG
                var bytes = tex.EncodeToPNG();
                Destroy(tex);

                string clipName = System.DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
                System.IO.File.WriteAllBytes(string.Format("{0}/{1}_({3},{4}).{2}", Application.persistentDataPath, clipName, "png", x, y), bytes);


            }
        }


    }
}
