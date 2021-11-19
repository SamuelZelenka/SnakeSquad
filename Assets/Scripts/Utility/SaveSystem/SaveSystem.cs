using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string SAVE_DIRECTORY = Application.dataPath + "/Save/";
    public static readonly string SAVEFILE_TYPE = ".json";

    public static void SaveData(SaveData data, string fileName)
    {
        if (!Directory.Exists(SAVE_DIRECTORY))
        {
            Directory.CreateDirectory(SAVE_DIRECTORY);
        }
        string json = JsonUtility.ToJson(data);
        string filePath = GetFilePath(fileName);
        File.WriteAllText(filePath, json);
    }

    public static T LoadData<T>(string fileName) where T : SaveData
    {
        string data = GetDataString(fileName);
        return JsonUtility.FromJson<T>(data);
    }

    public static string GetDataString(string fileName)
    {
        string filePath = GetFilePath(fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        return null;
    }
    private static string GetFilePath(string fileName)
    {
        return fileName = SAVE_DIRECTORY + fileName + SAVEFILE_TYPE;
    }
}
