using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton + classes
    public static NetworkManager instance;
    void Awake() { instance = this; }
    [System.Serializable]
    public class PlayerInfo
    {
        //public Side side;
        //public int actorNum;
        public int InsideNum;
        public int Health;
        public Transform Player;
    }
    #endregion

    public static Transform Spawn;
    public bool DebugScript = false;
    public List<NetworkPlayer> Players = new List<NetworkPlayer>();
    public List<PlayerInfo> info = new List<PlayerInfo>();
    public int InGame;

    public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();
    void Start()
    {
        if(InfoSave.instance.SceneState == SceneSettings.Public)
        {
            ConnectToServer();
        }
        Spawn = GameObject.Find("/Objects/Emptys/Spawn").transform;
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
    private void Update()
    {
        InGame = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            info[i].InsideNum = i;
            info[i].Player = Players[i].transform;
            info[i].Health = info[i].Player.GetComponent<PlayerControl>().Health;
            int PlayerNum = i + 1;
            Health[i].text = "Player " + PlayerNum + ": " + info[i].Player.GetComponent<PlayerControl>().Health + "/" + info[i].Player.GetComponent<PlayerControl>().MaxHealth;
        }
    }
}
