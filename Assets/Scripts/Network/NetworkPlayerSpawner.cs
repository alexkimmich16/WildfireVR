using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public static NetworkPlayerSpawner instance;
    void Awake() { instance = this; }
    public GameObject SpawnedPlayerPrefab;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, Quaternion.Euler(0,0,0));
        SpawnedPlayerPrefab.name = "My Player";

        //get player side

        if (SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            NetworkManager.instance.PlayerPhotonViews.Add(SpawnedPlayerPrefab.GetComponent<PhotonView>());
        }

        
    }
    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();

        NetworkManager.instance.PlayerPhotonViews.Remove(SpawnedPlayerPrefab.GetComponent<PhotonView>());
        PhotonNetwork.Destroy(SpawnedPlayerPrefab);
    }

    //override void ON
}
