using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SceneSettings
{
    Public = 0,
    Private = 1,
}
public enum Priority
{
    None = 0,
    Basic = 1,
    Override = 2,
}
public class InfoSave : MonoBehaviour
{
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
    public Priority priority;
    public static Priority HighestPriority = Priority.None;
}
