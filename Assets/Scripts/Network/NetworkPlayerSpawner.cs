using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject SpawnedPlayerPrefab;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        SpawnedPlayerPrefab.name = "My Player";

        //get player side

        if (SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {

        }
    }
    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        

        PhotonNetwork.Destroy(SpawnedPlayerPrefab);
    }

    //override void ON
}
