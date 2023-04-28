using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PhotonDestroy : MonoBehaviour
{
   
    public float LifeTime;
    public float Timer;
    private bool CountdownIsActive;
    public delegate void OnDestory();
    public event OnDestory DestoryEvent;
    public bool StartCountdownOnStart = false;
    public void StartCountdown()
    {
        CountdownIsActive = true;
    }
    public void DoDestroy()
    {
        DestoryEvent?.Invoke();

        if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine)
            DestroyOnline();
    }
    [PunRPC]
    public void DestroyOnline()
    {
        if(GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    void Update()
    {
        if(transform.position == Vector3.zero)
            gameObject.SetActive(false);
        
        if (!CountdownIsActive)
            return;
        Timer += Time.deltaTime;
        if (Timer > LifeTime)
            DoDestroy();
    }
    private void OnDisable()
    {
        Timer = 0f;
        CountdownIsActive = false;
    }
    private void OnEnable()
    {
        if(StartCountdownOnStart)
            CountdownIsActive = true;
    }
    
    private void Start()
    {
        //gameObject.SetActive(false);
        /*
        if (GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("ISmine");
            PhotonNetwork.Destroy(gameObject);
        }
            */
    }
}