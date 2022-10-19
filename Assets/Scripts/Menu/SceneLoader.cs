using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public enum CurrentGame
{
    StartMenu = 0,
    Testing = 1,
    Battle = 2,
}
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
    public static int SingletonNum = 1;
    public Priority priority;
    public static Priority HighestPriority = Priority.None;
    public List<GameObject> DestroyList = new List<GameObject>();

    public CurrentGame CurrentSetting;
    //final and unchangeable on this scene

    public static bool BattleScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Start()
    {
        CurrentSetting = (CurrentGame)SceneManager.GetActiveScene().buildIndex;
        SearchForInstance();
    }
    public void SearchForInstance()
    {
        if(InfoSave.instance != null)
            DestroyList.Add(InfoSave.instance.gameObject);
        if (SoundManager.instance != null)
            DestroyList.Add(SoundManager.instance.gameObject);
        if (SceneLoader.instance != null)
            DestroyList.Add(SceneLoader.instance.gameObject);
        
    }
    public void LoadScene(int Num)
    {
        for (int i = 0; i < DestroyList.Count; i++)
        {
            DontDestroyOnLoad(DestroyList[i]);
        }
        CurrentSetting = (CurrentGame)Num;
        SceneManager.LoadScene(Num);
    }
}