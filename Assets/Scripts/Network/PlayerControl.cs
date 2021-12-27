using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public int Health;
    public int MaxHealth;
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
        yield return new WaitForSeconds(1);
        Health = MaxHealth;
        transform.position = NetworkManager.Spawn.position;
    }
}
/*
        //int InRoom = PhotonNetwork.CountOfPlayers;
        int MyNum = transform.GetComponent<PhotonView>().ControllerActorNr;
        Transform mainPlayer = PhotonView.Find(MyNum).gameObject.transform;
        mainPlayer.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //PhotonNetwork.PlayerList[i].ActorNumber;
            if (NetworkManager.instance.Players[i].photonView.IsMine)
            {
                NetworkManager.instance.Players[i].UpdateHealth(Health);
            }
        }
        */