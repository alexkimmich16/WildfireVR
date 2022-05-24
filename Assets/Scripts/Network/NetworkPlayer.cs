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
    public PhotonView photonView;

    private Transform RigHead;
    private Transform RigLeft;
    private Transform RigRight;

    public Hashtable Info;

    public GameObject SkinRenderer;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XRRig rig = FindObjectOfType<XRRig>();
        RigHead = rig.transform.Find("Camera Offset/Main Camera");
        RigLeft = rig.transform.Find("Camera Offset/LeftHand Controller");
        RigRight = rig.transform.Find("Camera Offset/RightHand Controller");
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(false);
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);
            SkinRenderer.SetActive(false);

            MapPosition(Head, RigHead);
            MapPosition(Left, RigLeft);
            MapPosition(Right, RigRight);
        }
        if(SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            if (transform.position.x < ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
            {
                if (!Contains(ZoneController.instance.Players1, gameObject))
                {
                    ZoneController.instance.Players1.Add(gameObject);
                }
            }
            else if(transform.position.x > ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
            {
                if (!Contains(ZoneController.instance.Players2, gameObject))
                {
                    ZoneController.instance.Players2.Add(gameObject);
                }
            }
        }
        
        bool Contains(List<GameObject> AllObjects, GameObject myObject)
        {
            for (int i = 0; i < AllObjects.Count; i++)
                if (AllObjects[i] == myObject)
                    return true;
            return false;
        }
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
}
