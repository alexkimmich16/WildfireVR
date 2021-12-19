using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    #region Singleton + Classes
    public static MenuController instance;
    void Awake() { instance = this; }

    [System.Serializable]
    public class SceneInfo
    {
        public string SceneName;
        public bool FinalInfo;
    }
    #endregion

    public void JoinPublicLobby()
    {
        InfoSave.instance.SceneState = SceneSettings.Public;
        SceneLoader.instance.LoadScene(0);
    }
    public void JoinPrivateLobby()
    {
        InfoSave.instance.SceneState = SceneSettings.Private;
        SceneLoader.instance.LoadScene(0);
    }
}
