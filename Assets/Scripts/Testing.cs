using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Testing : SerializedMonoBehaviour
{
    public bool IsTesting;
    [ShowIf("IsTesting")]public bool SpawnInArena;
    [ShowIf("IsTesting")]public bool StopFadeIn;


    [FoldoutGroup("Ref")] public SpawnManager spawnManager;
    [FoldoutGroup("Ref")] public FadeManager fadeManager;
    private void Awake()
    {
        if (!IsTesting)
            return;


        if (SpawnInArena)
        {
            spawnManager.KeepInArena = true;
        }
        if (StopFadeIn)
        {
            fadeManager.enabled = false;
        }
    }
}
