using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class DataSaving : MonoBehaviour
{
    [System.Serializable]
    public class DataSavefile
    {
        public string FileName;
        public long maxValue = 0;
        public long totalValue = 0;
        public string maxIndex = "";
        public List<pointData> points;
    }

    [System.Serializable]
    public class pointData
    {
        public string coordinate;
        public long value;

        public pointData()
        {

        }

        public pointData(string coordinate, long value)
        {
            this.coordinate = coordinate;
            this.value = value;
        }
    }

    public DataSavefile _saveFile;
    private System.Diagnostics.Stopwatch watch;
    private float maxFrameLength = 0.01666f;
    private long tickBudget;

    private void Start()
    {

        watch = new System.Diagnostics.Stopwatch();
        tickBudget = (long)(System.Diagnostics.Stopwatch.Frequency
                                 * ((maxFrameLength)));
    }

    public long GetMaxValueInSaveFile()
    {
        return _saveFile.maxValue;
    }

    public void Save()
    {
        SaveFile("Data", _saveFile.FileName);
    }

    public void SetSaveFilePoints(Dictionary<string, long> points)
    {
        _saveFile.points = new List<pointData>();
        foreach (KeyValuePair<string, long> p in points)
        {
            _saveFile.points.Add(new pointData(p.Key, p.Value));
        }
    }

    public void SaveFile(string directoryName, string savingFileName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + directoryName + "/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directoryName + "/");
        }
        _saveFile.FileName = savingFileName;
        string dataFile = JsonUtility.ToJson(_saveFile);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + directoryName + "/" + savingFileName + ".json", dataFile);

        Debug.Log("Saved point file to " + Application.persistentDataPath + "/" + directoryName + "/" + savingFileName + ".json");
    }

    public void LoadFile(string directoryName, string savingFileName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + directoryName + "/"))
        {
            Debug.LogError("Directory does not exist: " + Application.persistentDataPath + "/" + directoryName + "/");
        }

        if (!System.IO.File.Exists(Application.persistentDataPath + "/" + directoryName + "/" + savingFileName + ".json"))
        {
            Debug.LogError("File does not exist: " + Application.persistentDataPath + "/" + directoryName + "/" + savingFileName + ".json");
        }

        string fileContent = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + directoryName + "/" + savingFileName + ".json");

        _saveFile = JsonUtility.FromJson<DataSavefile>(fileContent);
    }

    public void LoadFiles(string directoryName, string[] fileNames, float[] weights = null)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + directoryName + "/"))
        {
            Debug.LogError("Directory does not exist: " + Application.persistentDataPath + "/" + directoryName + "/");
        }

        if (weights == null)
        {
            weights = new float[fileNames.Length];
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 1;
        }

        Debug.Log("Loading loaded file into object.");

        _saveFile = new DataSavefile();
        Dictionary<string, short> keyValuePairs = new Dictionary<string, short>();

        int weightIndex = 0;

        foreach (string savingFileName in fileNames)
        {
            if (!System.IO.File.Exists(savingFileName))
            {
                Debug.LogError("File does not exist: " + savingFileName);
                continue;
            }

            string fileContent = System.IO.File.ReadAllText(savingFileName);

            var temp = JsonUtility.FromJson<DataSavefile>(fileContent);

            foreach (var p in temp.points)
            {
                _saveFile.points.Add(p);
            }

            weightIndex++;
        }

    }
}
