using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public int Health;
    public int MaxHealth;
    public static float DeathTime = 3f;
    public delegate void DisolveAll();
    public static event DisolveAll disolveEvent;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync health
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            //reading
            Health = (int)stream.ReceiveNext();
        }
    }
    public void ChangeHealth(int Change)
    {
        Health -= Change;
        
        if (Health < 1)
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        disolveEvent();
        yield return new WaitForSeconds(DeathTime);
        Health = MaxHealth;
        if(HandMagic.Respawn == true)
            HandMagic.instance.Cam.parent.parent.position = NetworkManager.instance.Spawn.position;
    }
}