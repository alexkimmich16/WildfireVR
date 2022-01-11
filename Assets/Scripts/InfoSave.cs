using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Attack = 0,
    Defense = 1,
}
public class InfoSave : MonoBehaviour
{
    public static int SingletonNum = 0;
    public Priority priority;
    public static Priority HighestPriority = Priority.None;
    public static bool Changeable = true;
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

    [Header("Info")]
    public Team team;
}
