using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveScript
{
    static bool DoDebug = false;
    public static string Path()
    {
        return Application.dataPath + "/player.fun";
    }
    public static void SaveStats()
    {
        if(DoDebug == true)
        {
            Debug.Log("save");
        }
        BinaryFormatter formatter = new BinaryFormatter();
        //Application.dataPath;
        FileStream stream = new FileStream(Path(), FileMode.Create);
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

        HandDebug.instance.LoadChildScriptableObjects(CurrentData);
        HandMagic.instance.LoadMainScriptableObjects(CurrentData);
    }

    public static AllData LoadGame()
    {
        FileStream stream = new FileStream(Path(), FileMode.Open);
        if (File.Exists(Path()) && stream.Length > 0)
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
                Debug.Log("NotFound in" + Path());
            }
            return null;
        }
    }
}
