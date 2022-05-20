using System.Collections;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
//using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public int Health;
    public static int MaxHealth = 100;
    public static float DeathTime = 1f;
    public delegate void DisolveAll();
    public event DisolveAll disolveEvent;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(Health);
        else
            Health = (int)stream.ReceiveNext();
    }
    public void ChangeHealth(int Change)
    {
        int oldHealth = GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer);
        int newHealth = oldHealth - Change;
        SetPlayerInt(PlayerHealth, newHealth, PhotonNetwork.LocalPlayer);
        if (newHealth < 1)
        {
            Death(true);
            //rpcDeathRPC()
        }
        
    }
    IEnumerator Respawn()
    {
        disolveEvent();
        yield return new WaitForSeconds(DeathTime);
        Health = MaxHealth;

        //enable ragdoll mode
        //seperate from player
        if(HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Testing)
        {
            HandMagic.instance.RB.transform.position = HandDebug.instance.Spawn.position;
        } 
        else if (HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            Transform Spawn = InGameManager.instance.SpectatorSpawns[Random.Range(0, InGameManager.instance.SpectatorSpawns.Count)];
            HandMagic.instance.RB.transform.position = Spawn.position;
        }
    }
    public void Death(bool IsMine)
    {
        StartCoroutine(Respawn());
        if(IsMine == true)
        {
            SetPlayerBool(PlayerAlive, false, PhotonNetwork.LocalPlayer);
        }
            

    }
    [PunRPC]
    public void DeathRPC()
    {
        Death(false);
    }
}