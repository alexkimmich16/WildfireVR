using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    public List<Eye> Eyes;
    public Material Fire;
    public Material Block;
    public Material None;
    public void ChangeEyes(int Type)
    {
        if (Type == 1)
            SetEyesNormal();
        else if (Type == 2 || Type == 1)
            SetEyesFire();
    }
    public void SetEyesNormal()
    {
        for (var i = 0; i < Eyes.Count; i++)
            Eyes[i].OBJ.GetComponent<SkinnedMeshRenderer>().material = None;
    }
    public void SetEyesFire()
    {
        for (var i = 0; i < Eyes.Count; i++)
            Eyes[i].OBJ.GetComponent<SkinnedMeshRenderer>().material = Fire;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //subtract
    }
}

[System.Serializable]
public class Eye
{
    public GameObject OBJ;
}
