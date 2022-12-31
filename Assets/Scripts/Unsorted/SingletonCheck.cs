using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonCheck : MonoBehaviour
{
    void Awake()
    {
        /*
        if ((int)priority > (int)HighestPriority)
        {
            HighestPriority = priority;
            instance = this;
        }
        else
            Destroy(this);
        */
    }

    [System.Serializable]
    public class ClassStats
    {
        public string ScriptName;
        public Priority Highest;
    }
    
    public List<ClassStats> Classes = new List<ClassStats>();
    /*
    public void CheckHighest(Priority priority, int Num, MonoBehaviour script)
    {
        //check
        if ((int)priority > (int)Classes[Num].Highest)
        {
            Classes[Num].Highest = priority;
            //instance = this;
        }
        else
        {
            Destroy(script);
            return;
        }
            
        //wasn't destoryed, so set as instance
        if (Num == 0)
        {
            InfoSave info = script.GetComponent<InfoSave>();
            InfoSave.instance = info;
        }
        else if (Num == 1)
        {
            SceneLoader info = script.GetComponent<SceneLoader>();
            SceneLoader.instance = info;
        }
        else if (Num == 2)
        {
            AudioController info = script.GetComponent<AudioController>();
            AudioController.instance = info;
        }
        else if (Num == 3)
        {
            InfoSave info = script.GetComponent<InfoSave>();
            InfoSave.instance = info;
        }
        /*
        if (Classes[Num].ScriptName == script.GetType().ToString())
        {
            //priority
            //(gameObject.GetComponent(MyScriptNames[i]) as MonoBehaviour).enabled = true;
        }
        
    }
    */
}
public enum Priority
{
    None = 0,
    Basic = 1,
    Override = 2,
    Final = 3,
}
