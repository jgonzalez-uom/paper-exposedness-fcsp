using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorRenderer : MonoBehaviour
{
    public MeshRenderer rendererMesh;
    public Gradient colorRange;
    public Texture2D referenceTexture;
    public long maxValue;

    public void Render(Dictionary<string, long> data, string highestIndex, long highestValue, string filename)
    {
        Debug.Log("\"" + highestIndex + "\": " + highestValue);
        Debug.Log("Rendering...");
        Texture2D newTexture = new Texture2D(rendererMesh.material.mainTexture.width, rendererMesh.material.mainTexture.height, TextureFormat.RGBA32, false, false, true);
        newTexture.filterMode = FilterMode.Point;

        //TestTexture(newTexture);
        //return;

        if (highestValue == 0)
        {
            Debug.LogError("Highest value was 0.");
        }

        byte squaresPerColumn = (byte)(newTexture.width / 255);
        int squareX, squareY;

        //Debug.Log(data.Count);
        bool testBool = false;

        if (maxValue > 0)
        {
            highestValue = maxValue;
        }

        for (int x = 0; x < referenceTexture.width; x++)
        {
            for (int y = 0; y < referenceTexture.height; y++)
            {
                string key = string.Format("{0},{1}", x, y);
                Color32 newColor = colorRange.Evaluate(0);

                if (data.ContainsKey(key))
                {

                    Debug.Log(string.Format("{0} = {1} ({2}/{3})", key, (float)data[key] / highestValue, data[key], highestValue));

                    newColor = colorRange.Evaluate((float)data[key] / highestValue);
                }

                newTexture.SetPixel(x, y, newColor);
            }
        }

        //foreach (KeyValuePair<string, long> pair in data)
        //{
        //    int x, y;
        //    ColorKey color = new ColorKey(pair.Key);
        //    squareX = color.b % squaresPerColumn;
        //    squareY = color.b / squaresPerColumn;

        //    x = squareX * 255 + color.r;
        //    y = squareY * 255 + color.g;

        //    Debug.Log(color + " = " + x + " , " + y + " : " + pair.Value);

        //    //newTexture.SetPixel(x, newTexture.height - 1 - y, colorRange.Evaluate(pair.Value / data[highestIndex]));

        //    //if (pair.Value > 0)
        //    //    Debug.Log(colorRange.Evaluate(pair.Value / highestValue));

        //    //if (!testBool)
        //    //{
        //    //    Debug.Log(string.Format("{0} -> {1},{2}", pair.Key, x, y));
        //    //    testBool = true;
        //    //}
        //}

        newTexture.Apply();
        Debug.Log("Displaying...");
        rendererMesh.material.mainTexture = newTexture;

        byte[] test = ImageConversion.EncodeToPNG(newTexture);
        System.IO.File.WriteAllBytes(Application.dataPath + "/"+ filename +".png", test);

        Debug.Log("Displayed.");

    }
}
