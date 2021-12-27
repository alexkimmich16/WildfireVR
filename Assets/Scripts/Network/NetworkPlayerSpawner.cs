using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject SpawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        NetworkManager.instance.Players.Add(SpawnedPlayerPrefab.GetComponent<NetworkPlayer>());
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        NetworkManager.instance.Players.Remove(SpawnedPlayerPrefab.GetComponent<NetworkPlayer>());
        PhotonNetwork.Destroy(SpawnedPlayerPrefab);
        
    }
}
