using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(GameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
#if UNITY_EDITOR
        string path = Application.persistentDataPath + "/dataPoint.bin";
#else
        string path = Application.persistentDataPath + "/dataPoint.fun";
#endif
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();
    }

    public static GameData LoadData()
    {
#if UNITY_EDITOR
        string path = Application.persistentDataPath + "/dataPoint.bin";
#else
        string path = Application.persistentDataPath + "/dataPoint.fun";
#endif
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return new GameData();
        }
    }
}