using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveScript
{
    public static void SaveStats()
    {
        //Debug.Log("save");
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
            //Debug.Log("Error");
            return;
        }

        UpgradeScript.ReciveLoad(CurrentData.Silver, CurrentData.Gems, CurrentData.Jump, CurrentData.Control, CurrentData.Speed);

    }

    public static AllData LoadGame()
    {
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Open);
        if (File.Exists(path) && stream.Length > 0)
        {
            //Debug.Log("Exists");

            BinaryFormatter formatter = new BinaryFormatter();
            AllData data = formatter.Deserialize(stream) as AllData;
            stream.Close();
            return data;
        }
        else
        {
            //Debug.Log("NotFound in" + path);
            return null;
        }
    }
}
