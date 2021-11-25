using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveScript
{

    static bool DoDebug = true;
    public static void SaveStats()
    {
        if(DoDebug == true)
        {
            Debug.Log("save");
        }
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        AllData data = new AllData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadGameLarge()
    {
        AllData CurrentData = LoadGame();
        if (CurrentData == null)
        {
            if (DoDebug == true)
            {
                Debug.Log("Error");
            }
            return;
        }
        HandDebug.instance.LoadScriptableObjects(CurrentData);
    }

    public static AllData LoadGame()
    {
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Open);
        if (File.Exists(path) && stream.Length > 0)
        {
            if (DoDebug == true)
            {
                Debug.Log("Exists");
            }
            BinaryFormatter formatter = new BinaryFormatter();
            AllData data = formatter.Deserialize(stream) as AllData;
            stream.Close();
            return data;
        }
        else
        {
            if (DoDebug == true)
            {
                Debug.Log("NotFound in" + path);
            }
            return null;
        }
    }
}
