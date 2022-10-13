using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkPlayer : MonoBehaviourPun
{
    public Transform Head;
    public Transform Left;
    public Transform Right;

    private Transform RigHead;
    private Transform RigLeft;
    private Transform RigRight;

    public GameObject SkinRenderer;
    public bool Testing;

    public delegate void StateEvent();
    public static event StateEvent TakeDamage;
    //public XR
    public static void TakeDamageMethod()
    {
        TakeDamage();
    }
    void Start()
    {
        LocomotionSystem rig = FindObjectOfType<LocomotionSystem>();
        RigHead = rig.transform.Find("Camera Offset/Main Camera");
        RigLeft = rig.transform.Find("Camera Offset/LeftHand Controller");
        RigRight = rig.transform.Find("Camera Offset/RightHand Controller");
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(Testing);
            Left.gameObject.SetActive(Testing);
            Right.gameObject.SetActive(Testing);
            SkinRenderer.SetActive(Testing);

            MapPosition(Head, RigHead);
            MapPosition(Left, RigLeft);
            MapPosition(Right, RigRight);
        }
        if(SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
            if (transform.position.x < ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
            {
                if (!Contains(ZoneController.instance.Players1, gameObject))
                    ZoneController.instance.Players1.Add(gameObject);
            }
            else if (transform.position.x > ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
                if (!Contains(ZoneController.instance.Players2, gameObject))
                    ZoneController.instance.Players2.Add(gameObject);
    }

    void MapPosition(Transform target,Transform rigTrans)
    {
        target.position = rigTrans.position;
        target.rotation = rigTrans.rotation;
    }

    public void RespawnAll()
    {
        photonView.RPC("FindSpotRPC", RpcTarget.All);
        //Team team = 

    }

    [PunRPC]
    void FindSpotRPC()
    {
        //reset my position
        DoorManager.instance.UpdateElevator();
        Debug.Log("RPC");
        BillBoardManager.instance.SetResetButton(false);
        Team team = GetPlayerTeam(PhotonNetwork.LocalPlayer);
        SpawnPoint SpawnInfo = InGameManager.instance.FindSpawn(team);
        InGameManager.instance.SetNewPosition(SpawnInfo);
        Debug.Log("newpos: " + SpawnInfo.ListNum);
        SetPlayerInt(PlayerSpawn, SpawnInfo.ListNum, PhotonNetwork.LocalPlayer);
        Debug.Log("RPC Respawn at: " + SpawnInfo.ListNum + " Team: " + team.ToString());
        //InGameManager.instance.FoundSpawn = true;
    }

    [PunRPC]
    public void takeDamage(int ID, int damg)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == ID)
        {
            NetworkManager.instance.LocalTakeDamage(damg);
            Debug.Log("Take Damage");
            TakeDamage();
        }
    }
}
