using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    public VFXHolder VFX;
    public FlameCollision FlameCol;
    private void Start()
    {
        GetComponent<PhotonDestroy>().DestoryEvent += OnDestory;
    }
    public void OnDestory()
    {

    }
    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        VFX.SetNewState(NewState);
    }

    public void SetFlames(bool NewState)
    {
        if (GetComponent<PhotonView>())
        {
            GetComponent<PhotonView>().RPC("SetFlamesOnline", RpcTarget.Others, NewState);
            VFX.SetNewState(false);
        }
        else
            VFX.SetNewState(NewState);

        if (NewState == false)
            GetComponent<PhotonDestroy>().StartCountdown();
    }
}
