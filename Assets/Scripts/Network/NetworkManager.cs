
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
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
        public Player player;
        //public int Num;
        public NetworkPlayer networkPlayer;
        public PlayerControl Control;
        public Transform ObjectReference;
    }

    #endregion
    static int MaxHealth = 100;
    public bool DebugScript = false;
    //public List<PlayerInfo> info = new List<PlayerInfo>();
    public List<PlayerStats> Players = new List<PlayerStats>();
    public int InGame;
    void Start()
    {
        ConnectToServer();
    }
    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        if (DebugScript == true)
        {
            Debug.Log("try connect to server");
        }
    }

    public override void OnConnectedToMaster()
    {
        if (DebugScript == true)
            Debug.Log("connected to server");
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
            Debug.Log("joined a room");
        InGameManager.instance.InitialisePlayer();
        if (SceneLoader.BattleScene() == true)
        {
            if (Initialized() == false)
            {
                SetGameFloat(GameWarmupTimer, 0f);
                SetGameFloat(GameFinishTimer, 0f);
                SetGameState(GameState.Waiting);

                SetGameBool(AttackSpawns[0], false);
                SetGameBool(AttackSpawns[1], false);
                SetGameBool(AttackSpawns[2], false);
                SetGameBool(DefenseSpawns[0], false);
                SetGameBool(DefenseSpawns[1], false);
                SetGameBool(DefenseSpawns[2], false);

                SetGameInt(AttackTeamCount, 0);
                SetGameInt(DefenseTeamCount, 0);

                Debug.Log("Initialized room and reset or created all stats");
                
            }
        }
        base.OnJoinedRoom();
        InGameManager.instance.ReCalculateTeamSize();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (DebugScript == true)
            Debug.Log("a new player joined");
        base.OnPlayerEnteredRoom(newPlayer);
    }
    private void Update()
    {
        InGame = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int PlayerNum = i + 1;
            if (SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
            {
                //BillBoardManager.instance.Health[i].text = "Player " + PlayerNum + ": " + info[i].Player.GetComponent<PlayerControl>().Health + "/" + info[i].Player.GetComponent<PlayerControl>().MaxHealth;
            }
            else if(SceneLoader.instance.CurrentSetting == CurrentGame.Testing)
            {
                //HandDebug.instance.Health[i].text = "Player " + PlayerNum + ": " + info[i].Player.GetComponent<PlayerControl>().Health + "/" + info[i].Player.GetComponent<PlayerControl>().MaxHealth;
            }
        }
        
    }

    
}
