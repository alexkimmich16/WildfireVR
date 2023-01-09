using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using static Odin.Net;
using Photon.Pun;
public class BlockObject : MonoBehaviour
{
    public VFXHolder VFX;
    [PunRPC]
    public void SetOnlineVFX(bool State)
    {
        VFX.SetNewState(State);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
