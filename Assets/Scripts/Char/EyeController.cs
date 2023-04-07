using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public enum Eyes
{
    None = 0,
    Block = 1,
    Fire = 2,
}

//rpc call to eyec
public class EyeController : MonoBehaviour
{
    public static EyeController instance;
    void Awake() { instance = this; }
    public float ColorTime;
    private float TimeLeft;
    public List<GameObject> AllEyes;
    public List<Material> EyeMats;
    public bool AllowEyeChange;
    public void ChangeEyes(Eyes eyes)
    {
        if(eyes != Eyes.None)
            TimeLeft = ColorTime;
        for (var i = 0; i < AllEyes.Count; i++)
            AllEyes[i].GetComponent<SkinnedMeshRenderer>().material = EyeMats[(int)eyes];
        if(NetworkPlayerSpawner.instance.SpawnedPlayerPrefab != null && AllowEyeChange)
            NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("SetEyes", RpcTarget.All, eyes);
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeLeft > 0)
            TimeLeft -= Time.deltaTime;
        else
        {
            ChangeEyes(Eyes.None);
        }
    }
}