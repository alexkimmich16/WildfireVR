using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Singleton
    public static SceneLoader instance;
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

    public List<SceneInfo> Scenes = new List<SceneInfo>();
    public Priority priority;
    public static Priority HighestPriority = Priority.None;

    public void LoadScene(int Num)
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(Scenes[Num].SceneName);
    }

    [System.Serializable]
    public class SceneInfo
    {
        public string SceneName;
    }
}
