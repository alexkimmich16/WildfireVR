using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public bool DebugScript = false;
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }
    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        if(DebugScript == true)
        {
            Debug.Log("try connect to server");
        }
    }
    public override void OnConnectedToMaster()
    {
        if (DebugScript == true)
        {
            Debug.Log("connected to server");
        }
        
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;


        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (DebugScript == true)
        {
            Debug.Log("joined a room");
        }
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (DebugScript == true)
        {
            Debug.Log("a new player joined");
        }
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
