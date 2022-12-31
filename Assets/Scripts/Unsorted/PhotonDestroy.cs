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
    public void StartCountdown()
    {
        IsActive = true;
    }
    public void DoDestroy()
    {
        if (DestoryEvent != null)
            DestoryEvent();
        if (GetComponent<PhotonView>() != null)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }
    void Update()
    {
        if (!IsActive)
            return;
        Timer += Time.deltaTime;
        if (Timer > LifeTime)
            DoDestroy();
    }
}
//Int myVariable = ( someBoolValueOrStatementHere ) ? 1 : 0;