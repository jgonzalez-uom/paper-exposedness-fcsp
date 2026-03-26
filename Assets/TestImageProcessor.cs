using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class TestImageProcessor : MonoBehaviour
{
    public ColorRenderer textureEncoder;
    public PartsAnalysisScript partsAnalyzer;
    public Dictionary<string, long> indexedColors = new Dictionary<string, long>();
    public DataSaving dataSavingObject;
    public string pPathFolderName;
    public string fileName;
    private Texture2D[] data;
    public Color32[] ignoreColorsNoAlpha;
    public bool registerColorOncePerImage;
    public bool analysisStarted = false;
    public TMPro.TextMeshProUGUI progress;

    public Texture2D referenceTexture;
    public Dictionary<string, long> pixelValues = new Dictionary<string, long>();
    private long totalValue = 0;


    private void Start()
    {
        //StartCoroutine(AnalyzeAndDisplay());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !analysisStarted)
        {
            StartCoroutine(AnalyzeAndDisplay());
            analysisStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadFile();
        }
    }

    public IEnumerator AnalyzeAndDisplay()
    {
        Debug.Log("Finding files in " + Application.persistentDataPath+"/" + pPathFolderName + "...");
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/" + pPathFolderName);
        FileInfo[] files = dir.GetFiles("*.*");

        //data = Resources.LoadAll<Texture2D>(resourceFolderName);

        Debug.Log("Processing...");
        int count = 0;
        int partCount = 0;
        int target = files.Length;
        Texture2D texture = new Texture2D(1, 1);
        progress.text = string.Format("{0}/{1}", count, target);

        yield return new WaitForEndOfFrame();

        string tempS;
        string highestIndex = "";
        string secondHighestIndex = "";
        HashSet<string> ignoreString = new HashSet<string>();
        HashSet<string> ignoreStringNoAlpha = new HashSet<string>();
        HashSet<string> colorsSeen = new HashSet<string>();
        
        foreach (var c in ignoreColorsNoAlpha)
        {
            ignoreStringNoAlpha.Add(StringColor(c));
        }

        foreach (var file in files)
        {
            count++;

            byte[] bytes = File.ReadAllBytes(file.FullName);
            
            if (!ImageConversion.LoadImage(texture, bytes, false))
            {
                Debug.LogError("Couldn't load " + file.FullName + " into a texture.");
                continue;
            }

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color32 tempC = (Color32)texture.GetPixel(x, y);
                    tempC.a = 255;
                    tempS = StringColor(tempC);

                    if (!ignoreStringNoAlpha.Contains(tempS) && (!registerColorOncePerImage || colorsSeen.Add(tempS)))
                    {
                        if (indexedColors.ContainsKey(tempS))
                        {
                            indexedColors[tempS]++;
                        }
                        else
                        {
                            indexedColors.Add(tempS, 1);
                        }
                    }
                }
                progress.text = string.Format("{0}/{1} ({2}/{3})", count, target, x, texture.width);
                yield return null;
            }

            progress.text = string.Format("{0}/{1} ({2}/{3})", count, target, texture.width, texture.width);
            yield return null;

            if (registerColorOncePerImage)
                colorsSeen.Clear();
        }

        long highestValue = 0;
        totalValue = 0;

        Debug.Log("Finding Highest Value");
        yield return null;
        foreach (KeyValuePair<string, long> d in indexedColors)
        {
            if (d.Value > highestValue)
            {
                highestIndex = d.Key;
                highestValue = d.Value;
            }
            totalValue += d.Value;
        }
        Debug.Log("Registering Pixel Values");
        yield return null;

        for (int x = 0; x < referenceTexture.width; x++)
        {
            for (int y = 0; y < referenceTexture.height; y++)
            {
                Color32 color = referenceTexture.GetPixel(x, y);
                color.a = 255;
                string key = StringColor(color);
                long pixelVal = 0;

                if (indexedColors.TryGetValue(key, out pixelVal))
                {
                    pixelValues.Add(string.Format("{0},{1}", x, y), pixelVal);
                    //pixelValues[string.Format("{0},{1}", x, y)] = pixelVal;
                }
            }

            progress.text = string.Format("Processing Rows: {0}/{1}", x, referenceTexture.width);
            yield return null;
        }


        Debug.Log("Saving.");
        yield return null;
        dataSavingObject.SetSaveFilePoints(pixelValues);
        dataSavingObject._saveFile.maxIndex = highestIndex;
        dataSavingObject._saveFile.maxValue = highestValue;
        dataSavingObject._saveFile.FileName = fileName;
        dataSavingObject.Save();
        Debug.Log("Saved.");
        yield return null;
        textureEncoder.Render(pixelValues, highestIndex, highestValue, fileName);
        partsAnalyzer.ProcessPartData(pixelValues);
        partsAnalyzer.OutputPartStats(fileName + "_part_report");
    }

    string StringColor(Color32 color)
    {
        return string.Format("{0},{1},{2}", color.r, color.g, color.b);
    }

    //bool CompareC32(Color32 a, Color32 b)
    //{
    //    return a.r == b.r && a.g == b.g && a.b == b.b;
    //}

    void LoadFile()
    {
        dataSavingObject.LoadFile("Data", fileName);

        indexedColors = new Dictionary<string, long>();
        pixelValues = new Dictionary<string, long>();
        totalValue = dataSavingObject._saveFile.totalValue;

        Debug.Log("Loaded file...");


        foreach (var p in dataSavingObject._saveFile.points)
        {
            Debug.Log(p.coordinate);
            string[] vals = p.coordinate.Split(',');
            int x = int.Parse(vals[0]);
            int y = int.Parse(vals[1]);

            pixelValues.Add(p.coordinate, p.value); ;
            indexedColors.Add(StringColor(referenceTexture.GetPixel(x, y)), p.value);
        }

        textureEncoder.Render(pixelValues, dataSavingObject._saveFile.maxIndex, dataSavingObject._saveFile.maxValue, fileName);
        partsAnalyzer.ProcessPartData(pixelValues);
        partsAnalyzer.OutputPartStats(fileName + "_part_report");
    }
}
