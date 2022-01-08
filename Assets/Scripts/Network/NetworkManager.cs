
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton + classes
    public static NetworkManager instance;
    void Awake() { instance = this; }
    [System.Serializable]
    public class PlayerInfo
    {
        public int InsideNum;
        public int Health;
        public Transform Player;
    }
    [System.Serializable]
    public class PlayerStats
    {
        public NetworkPlayer networkPlayer;
        public PlayerControl Control;
        public Transform ObjectReference;
    }

    #endregion

 
    public bool DebugScript = false;
    public List<PlayerInfo> info = new List<PlayerInfo>();
    public List<PlayerStats> Players = new List<PlayerStats>();
    public int InGame;

    //public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();

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
    private void Update()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            info[i].InsideNum = i;
            info[i].Player = Players[i].networkPlayer.transform;
            info[i].Health = info[i].Player.GetComponent<PlayerControl>().Health;
            int PlayerNum = i + 1;
            if (HandDebug.instance != null)
                HandDebug.instance.Health[i].text = "Player " + PlayerNum + ": " + info[i].Player.GetComponent<PlayerControl>().Health + "/" + info[i].Player.GetComponent<PlayerControl>().MaxHealth;
        }
        InGame = PhotonNetwork.PlayerList.Length;
        
    }
}
