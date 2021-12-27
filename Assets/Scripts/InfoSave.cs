using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SceneSettings
{
    Public = 0,
    Private = 1,
}

public class InfoSave : MonoBehaviour
{
    public static int SingletonNum = 0;
    public Priority priority;
    public static Priority HighestPriority = Priority.None;
    #region Singleton + Classes
    public static InfoSave instance;
    void Awake()
    {
        if ((int)priority > (int)HighestPriority)
        {
            HighestPriority = priority;
            instance = this;
        }
        else
            Destroy(this);
    }
    #endregion
    public SceneSettings SceneState;
}
