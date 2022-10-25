using System.Collections;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
//using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerControl : MonoBehaviourPunCallbacks
{
    public int Health;
    public static int MaxHealth = 100;
    public static float DeathTime = 1f;
    public delegate void DisolveAll();
    public event DisolveAll disolveEvent;
    
    public IEnumerator DisolveRespawn()
    {
        disolveEvent();
        yield return new WaitForSeconds(DeathTime);
        Health = MaxHealth;

        //enable ragdoll mode
        //seperate from player
        /*
        if(HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Testing)
        {
            HandMagic.instance.RB.transform.position = HandDebug.instance.Spawn.position;
        } 
        else if (HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            Transform Spawn = InGameManager.instance.SpectatorSpawns[Random.Range(0, InGameManager.instance.SpectatorSpawns.Count)];
            HandMagic.instance.RB.transform.position = Spawn.position;
        }
        */
    }

    public IEnumerator RagdollRespawn()
    {
        gameObject.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
        //camera grey to floor(post processing)
        yield return new WaitForSeconds(DeathTime);
    }

    [PunRPC]
    public void SetEyes(Eyes eyes) { GetComponent<MultiplayerEyes>().ChangeEyes(eyes); }
    [PunRPC]
    public void MotionDone(Spell spell) { FirePillar.CallStartFire(spell); }
    [PunRPC]
    public void PlayerDied()
    {
        //ragdoll
        GetComponent<Ragdoll>().EnableRagdoll();
    }







    /*
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
            //Death(true);
            //rpcDeathRPC()
        }
        
    }
    */
}