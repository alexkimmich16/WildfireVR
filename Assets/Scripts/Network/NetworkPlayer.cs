using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using RestrictionSystem;
public class NetworkPlayer : MonoBehaviourPun
{
    public static float DeathTime = 1f;


    //public delegate void DisolveAll();
    //public event DisolveAll disolveEvent;

    public Transform Head;
    public Transform Left;
    public Transform Right;

    private Transform RigHead;
    private Transform RigLeft;
    private Transform RigRight;
    private Transform CharacterDisplay;

    public GameObject SkinRenderer;
    public bool TestingSelf;
    public bool Dead;

    public Ragdoll myRagdoll;

    private FMOD.Studio.EventInstance TakeDamageInstance;
    private FMOD.Studio.EventInstance DeathInstance;

    void Start()
    {
        OnlineEventManager.RestartEventCallback += OnReset;

        CharacterDisplay = AIMagicControl.instance.MyCharacterDisplay;
        RigHead = AIMagicControl.instance.CamOffset;
        RigLeft = AIMagicControl.instance.PositionObjectives[(int)Side.left];
        RigRight = AIMagicControl.instance.PositionObjectives[(int)Side.right];

        transform.SetParent(NetworkManager.instance.playerList);

        NetworkManager.OnTakeDamage += TakeDamage;
        NetworkManager.OnDeath += PlayerDied;
    }
    public void OnReset()
    {
        Dead = false;
        myRagdoll.DisableRagdoll();
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(TestingSelf || Dead);
            Left.gameObject.SetActive(TestingSelf || Dead);
            Right.gameObject.SetActive(TestingSelf || Dead);
            SkinRenderer.SetActive(TestingSelf || Dead);

            //MapPosition(transform, CharacterDisplay);
            if (!Dead)
            {
                transform.position = CharacterDisplay.position;
                MapPosition(Head, RigHead);
                MapPosition(Left, RigLeft);
                MapPosition(Right, RigRight);
            }
            
        }
        if (SceneLoader.instance.CurrentSetting != CurrentGame.Battle)
            return;
        //if()
    }

    void MapPosition(Transform target,Transform rigTrans)
    {
        target.position = rigTrans.position;
        target.rotation = rigTrans.rotation;
    }

    [PunRPC]
    public void PlayerDied()
    {
        //call for others
        if (photonView.IsMine)
            photonView.RPC("PlayerDied", RpcTarget.Others);

        Dead = true;
        myRagdoll.EnableRagdoll();
        if (SoundManager.instance.CanPlay(SoundType.Effect))
        {
            DeathInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.RandomSound(SoundManager.instance.DeathRef));
            DeathInstance.setVolume(SoundManager.instance.Volume(SoundType.Effect));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(DeathInstance, GetComponent<Transform>());
            DeathInstance.start();
        }
    }

    [PunRPC]
    public void TakeDamage()
    {
        if (photonView.IsMine)
            photonView.RPC("TakeDamage", RpcTarget.Others);

        if (SoundManager.instance.CanPlay(SoundType.Effect))
        {
            TakeDamageInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.RandomSound(SoundManager.instance.DamageRef));
            TakeDamageInstance.setVolume(SoundManager.instance.Volume(SoundType.Effect));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(TakeDamageInstance, GetComponent<Transform>());
            TakeDamageInstance.start();
        }
    }


    //[PunRPC]
    //public void MotionDone(RestrictionSystem.CurrentLearn spell) { FirePillar.CallStartFire(spell); }
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

/*
public IEnumerator DisolveRespawn()
{
    disolveEvent();
    yield return new WaitForSeconds(DeathTime);

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
*/