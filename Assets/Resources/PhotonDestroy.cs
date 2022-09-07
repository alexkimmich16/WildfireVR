using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PhotonDestroy : MonoBehaviour
{
    public float LifeTime;
    public float Timer;
    private bool IsActive;
    public void StartCountdown()
    {
        IsActive = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            return;
        Timer += Time.deltaTime;
        if(Timer > LifeTime)
        {
            //PhotonNetwork.Destroy(gameObject);
            Destroy(gameObject);
        }
    }
}
