
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
    public void InitializeRoom()
    {
        object temp;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Attack0", out temp))
        {
            if (temp is bool)
            {
                bool activeGame = (bool)temp;
                //Debug.Log(activeGame);
            }
        }
        if (DebugScript == true)
            Debug.Log("2set");
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

        Hashtable hash = new Hashtable();
        hash.Add("Attack0", false);
        hash.Add("Attack1", false);
        hash.Add("Attack2", false);
        hash.Add("Defense0", false);
        hash.Add("Defense1", false);
        hash.Add("Defense2", false);

        hash.Add("AttackTeam", 0);
        hash.Add("DefenseTeam", 0);

        roomOptions.CustomRoomProperties = hash;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        Team team = InfoSave.instance.team;
        Player local = PhotonNetwork.LocalPlayer;

        SetPlayerTeam("TEAM", team, local);
        SetPlayerInt("HEALTH", MaxHealth, local);
        SetPlayerInt("SpawnNum", 4, local);

        if (DebugScript == true)
            Debug.Log("joined a room");
        InitializeRoom();
        if (SceneLoader.BattleScene() == true)
            InGameManager.instance.Initialise();
        base.OnJoinedRoom();
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

    public int GetLocal()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
            {
                return i;
            }
        }
        Debug.LogError("Get Local Failure");
        return 100;
    }

    #region NetworkGet
    public static int GetRoomInt(string text)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
            return (int)temp;
        else
        {
            Debug.LogError("GetHash.GetInt.OfRoom with string: " + text + "has not been set");
            return 100;
        }
    }
    public static int GetPlayerInt(string text, Player player)
    {
        if (player.CustomProperties.TryGetValue(text, out object temp))
            return (int)temp;
        else
        {
            Debug.LogError("GetHash.GetInt.OfPlayer with string: " + text + "has not been set");
            return 100;
        }
    }
    public static bool GetRoomBool(string text)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
            return (bool)temp;
        else
        {
            Debug.LogError("GetHash.GetBool with string: " + text + "has not been set");
            return true;
        }
    }

    public static bool GetPlayerBool(string text, Player player)
    {
        if (player.CustomProperties.TryGetValue(text, out object temp))
            return (bool)temp;
        else
        {
            Debug.LogError("GetHash.GetBool with string: " + text + "has not been set");
            return true;
        }
    }
    #endregion

    #region NetworkSet
    public static void SetPlayerTeam(string text, Team team, Player player)
    {
        Hashtable TeamHash = new Hashtable();
        TeamHash.Add(text, team);
        player.SetCustomProperties(TeamHash);
    }
    public static void SetPlayerBool(string text, bool State, Player player)
    {
        Hashtable HealthHash = new Hashtable();
        HealthHash.Add(text, State);
        player.SetCustomProperties(HealthHash);
    }
    public static void SetRoomBool(string text, bool State)
    {
        Hashtable HealthHash = new Hashtable();
        HealthHash.Add(text, State);
        PhotonNetwork.CurrentRoom.SetCustomProperties(HealthHash);
    }
    public static void SetPlayerInt(string text, int SetNum, Player player)
    {
        Hashtable HealthHash = new Hashtable();
        HealthHash.Add(text, SetNum);
        player.SetCustomProperties(HealthHash);
    }
    public static void SetRoomInt(string text, int SetNum)
    {
        Hashtable HealthHash = new Hashtable();
        HealthHash.Add(text, SetNum);
        PhotonNetwork.CurrentRoom.SetCustomProperties(HealthHash);
    }
    #endregion
}
