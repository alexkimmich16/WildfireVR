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
    #region Singleton + Classes
    public static InfoSave instance;
    void Awake() { instance = this; }
    #endregion

    public SceneSettings SceneState;
}
