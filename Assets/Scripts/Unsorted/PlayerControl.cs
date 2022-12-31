using System.Collections;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
//using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerControl : MonoBehaviourPunCallbacks
{
    //public int Health;
    public static int MaxHealth = 100;
    public static float DeathTime = 1f;

    
    public delegate void DisolveAll();
    public event DisolveAll disolveEvent;

    private FMOD.Studio.EventInstance TakeDamageInstance;
    private FMOD.Studio.EventInstance DeathInstance;

    

    [PunRPC]
    public void SetEyes(Eyes eyes) { GetComponent<MultiplayerEyes>().ChangeEyes(eyes); }
    [PunRPC]
    public void MotionDone(Spell spell) { FirePillar.CallStartFire(spell); }
    [PunRPC]
    public void PlayerDied()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
        {
            DeathInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.DeathRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(DeathInstance, GetComponent<Transform>());
            DeathInstance.start();
        }
            
    }
    [PunRPC]
    public void TakeDamage()
    {
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
        {
            TakeDamageInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.TakeDamageRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(TakeDamageInstance, GetComponent<Transform>());
            TakeDamageInstance.start();
        }
        
    }

    public IEnumerator DisolveRespawn()
    {
        disolveEvent();
        yield return new WaitForSeconds(DeathTime);

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