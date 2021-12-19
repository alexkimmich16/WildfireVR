using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Singleton + Classes
    public static SceneLoader instance;
    void Awake() { instance = this; }

    [System.Serializable]
    public class SceneInfo
    {
        public string SceneName;
    }
    #endregion

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public void LoadScene(int Num)
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(Scenes[Num].SceneName);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
