using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    #region Singleton + Classes
    public static MenuController instance;
    void Awake()
    {
        instance = this;
    }
    #endregion
    
    public void JoinAttack()
    {
        InfoSave.instance.team = Team.Attack;
        Debug.Log(SceneLoader.instance.gameObject);
        SceneLoader.instance.LoadScene(2);
    }
    public void JoinDefense()
    {
        InfoSave.instance.team = Team.Defense;
        SceneLoader.instance.LoadScene(2);
    }
    
}
