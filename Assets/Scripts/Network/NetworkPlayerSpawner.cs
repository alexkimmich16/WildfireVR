using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject SpawnedPlayerPrefab;
    int PlayerCount;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        SpawnedPlayerPrefab.name = "Player " + PlayerCount;
        PlayerCount += 1;
        NetworkManager.instance.Players.Add(new NetworkManager.PlayerStats());
        int Count = NetworkManager.instance.Players.Count - 1;
        NetworkManager.instance.Players[Count].networkPlayer = SpawnedPlayerPrefab.GetComponent<NetworkPlayer>();
        NetworkManager.instance.Players[Count].Control = SpawnedPlayerPrefab.GetComponent<PlayerControl>();
        NetworkManager.instance.Players[Count].ObjectReference = SpawnedPlayerPrefab.transform;

        if (SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {

        }
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        for (int i = 0; i < NetworkManager.instance.Players.Count; i++)
        {
            if (NetworkManager.instance.Players[i].networkPlayer == SpawnedPlayerPrefab.GetComponent<NetworkPlayer>())
            {
                NetworkManager.instance.Players.Remove(NetworkManager.instance.Players[i]);
            }
        }
        PhotonNetwork.Destroy(SpawnedPlayerPrefab);
    }
}
