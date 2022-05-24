using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ForcepushManager : MonoBehaviour
{
    #region Singleton + classes
    public static ForcepushManager instance;
    void Awake() { instance = this; }
    #endregion

    public float PushAmount = 0;
    public float PushMax;
    public float PushFallMultiplier;
    public VisualEffect Force;
    private Vector3 pos;

    // Update is called once per frame
    void Update()
    {
        PushAmount -= Time.deltaTime * PushFallMultiplier;
        if (PushAmount < 0)
        {
            PushAmount = 0;
            Force.SetFloat("ForceMultiplyer", PushAmount);
            Force.SetInt("ShouldPush", 0);
        }
        else
        {
            Force.SetFloat("ForceMultiplyer", PushAmount);
            Force.SetInt("ShouldPush", 1);
            Force.SetVector3("WorldPosition", pos);
        }
        
        
    }

    public void Push(Vector3 position)
    {
        PushAmount = PushMax;
        pos = position;
    }
}
