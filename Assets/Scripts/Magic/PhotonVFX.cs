using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using static Odin.Net;
using Photon.Pun;
public class PhotonVFX : MonoBehaviour
{
    public VFXHolder VFX;
    [PunRPC]
    public void SetOnlineVFX(bool State)
    {
        VFX.SetNewState(State);
    }

    private void Start()
    {
        //VFX.SetNewState(false);
    }
}
