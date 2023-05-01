using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using static Odin.Net;
using Photon.Pun;
public class PhotonVFX : MonoBehaviour
{
    public delegate void SetVFXCallback(bool State);
    public event SetVFXCallback SetVFX;
    public VFXHolder VFX;
    [PunRPC]
    public void SetOnlineVFX(bool State)
    {
        VFX.SetNewState(State);
        SetVFX?.Invoke(State);
    }
}
