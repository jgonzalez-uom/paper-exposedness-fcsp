using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PartsAnalysisScript : MonoBehaviour
{
    public Texture2D partsReference;
    public long partValueTotal = 0;
    public Dictionary<string, partInfo> partsInfo = new Dictionary<string, partInfo>();
    public List<PartIdentification> partIDs;
    Dictionary<string, PartIdentification> dictIds = new Dictionary<string, PartIdentification>();
    long overallTotal;
    private string reportDirectory = "PartReports";

    public class partInfo
    {
        public string name;
        public long total = 0;
        public long highestValue = 0;
        public long pixelCount = 0;

        public partInfo()
        {
            total = 0;
            highestValue = 0;
            pixelCount = 0;
        }
    }

    [System.Serializable]
    public class PartIdentification
    {
        public string name;
        public Color32 color;
    }

    private void Start()
    {
        foreach (var p in partIDs)
        {
            //Debug.Log(p.name);
            dictIds.Add(StringColor(p.color), p);
        }
    }

    void GetPartPixelStats()
    {
        partsInfo = new Dictionary<string, partInfo>();
        overallTotal = 0;

        foreach (var p in partIDs)
        {
            partInfo t = new partInfo();
            t.name = p.name;
            partsInfo.Add(StringColor(p.color), t);

        }

        for (int x = 0; x < partsReference.width; x++)
        {
            for (int y = 0; y < partsReference.height; y++)
            {
                var tempR = StringColor(partsReference.GetPixel(x, y));

                if (!partsInfo.ContainsKey(tempR))
                {
                    continue;
                    partsInfo.Add(tempR, new partInfo());
                }

                partsInfo[tempR].pixelCount++;
            }
        }
    }

    public void ProcessPartData(Dictionary<string, long> data)
    {
        GetPartPixelStats();

        for (int x = 0; x < partsReference.width; x++)
        {
            for (int y = 0; y < partsReference.height; y++)
            {

                var tempR = StringColor(partsReference.GetPixel(x, y));
                var tempC = string.Format("{0},{1}", x, y);

                long dataVal = 0;

                if (!partsInfo.ContainsKey(tempR) || !data.TryGetValue(tempC, out dataVal))
                    continue;
                

                Debug.Log(string.Format("data[{0}] = {1} ({2})", tempC, dataVal, tempR));


                partsInfo[tempR].total += dataVal;
                overallTotal += dataVal;

                if (dataVal > partsInfo[tempR].highestValue)
                {
                    partsInfo[tempR].highestValue = dataVal;
                }
                Debug.Log(string.Format("partsInfo[{0}] = {1}", tempC, partsInfo[tempR].total));
            }
        }
    }

    public void OutputPartStats(string filename)
    {

        string report = "part_name,pixel_count,part_total_value,highest_value,average_value,total_percentage\n";
        foreach (KeyValuePair<string, partInfo> p in partsInfo)
        {
            var val = p.Value;
            Debug.Log(string.Format("{0},{1},{2}", val.pixelCount, val.highestValue, overallTotal));
            long pCount = (val.pixelCount > 0) ? val.pixelCount : 1;
            long hVal= (val.highestValue > 0) ? val.highestValue : 1;
            long oTot = (overallTotal > 0) ? overallTotal : 1;


            //report += string.Format("{0} ({5})\tCount: {1}\tValue: {6}\tHighest: {2}\tAverage: {3}\tPercentage: {4}\n", val.name, val.pixelCount, val.highestValue, (float)val.total / pCount, (float)val.total * 100 / oTot, p.Key, val.total);
            report += string.Format("{0},{1},{2},{3},{4},{5}\n", val.name, val.pixelCount, val.total, val.highestValue, (float)val.total / pCount, (float)val.total * 100 / oTot);
        }

        if (!Directory.Exists(Application.persistentDataPath + "/" + reportDirectory + "/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + reportDirectory + "/");
        }

        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + reportDirectory + "/" + filename + ".csv", report);
        Debug.Log(report);
    }

    string StringColor(Color32 color)
    {
        return string.Format("{0},{1},{2}", color.r, color.g, color.b);
    }
}
