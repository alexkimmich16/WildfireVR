using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PhotonDestroy : MonoBehaviour
{
   
    public float LifeTime;
    public float Timer;
    private bool IsActive;
    public delegate void OnDestory();
    public event OnDestory DestoryEvent;
    public bool StartCountdownOnStart = false;
    public void StartCountdown()
    {
        IsActive = true;
    }
    public void DoDestroy()
    {
        if (DestoryEvent != null)
            DestoryEvent();




        if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine)
            DestroyOnline();
        else if(PhotonNetwork.IsMasterClient)
            Destroy(gameObject);


        //if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine)
        //GetComponent<PhotonView>().RPC("DestroyOnline", RpcTarget.All);
    }
    [PunRPC]
    public void DestroyOnline()
    {
        if(GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    void Update()
    {
        if (!IsActive)
            return;
        Timer += Time.deltaTime;
        if (Timer > LifeTime)
            DoDestroy();
    }
    private void OnEnable()
    {
        if (StartCountdownOnStart)
            StartCountdown();
        IsActive = false;
        Timer = 0f;
    }
}
//Int myVariable = ( someBoolValueOrStatementHere ) ? 1 : 0;