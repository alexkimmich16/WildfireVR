
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
    //public List<PlayerStats> Players = new List<PlayerStats>();
    public int InGame;
    public List<PhotonView> PlayerPhotonViews;

    public delegate void DamageEvent(int Damage);
    public event DamageEvent OnTakeDamage;

    public delegate void initializeEvent();
    public static event initializeEvent OnInitialized;

    public delegate void Fade(bool In);
    public static event Fade DoFade;

    public float AfterDeathWait;

    public bool CanRecieveDamage = true;

    public List<GameObject> GetPlayers()
    {
        List<GameObject> Players = new List<GameObject>();
        for (int i = 0; i < playerList.childCount; ++i)
            Players.Add(playerList.GetChild(i).gameObject);
        return Players;
    }
    public List<GameObject> Players;
    public Transform playerList;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }
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
    public static bool HasConnected()
    {
        return PhotonNetwork.InRoom == true && Initialized();
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
   

    public IEnumerator WaitForInitalized()
    {
        yield return new WaitWhile(() => Initialized() == false);
        if(DebugScript)
            Debug.Log("init: " + Initialized());
        OnInitialized?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        if (DebugScript == true)
            Debug.Log("joined a room");

        SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.LocalPlayer);
        if (Initialized() == false)
        {
            InitializeAllGameStats();
        }
        StartCoroutine(WaitForInitalized());
        base.OnJoinedRoom();
        void InitializeAllGameStats()
        {
            //Debug.Log("set");
            
            SetGameFloat(GameWarmupTimer, 0f);
            SetGameFloat(GameFinishTimer, 0f);
            SetGameState(GameState.Waiting);

            SetGameInt(DoorState, (int)SequenceState.Waiting);
            
            for (int i = 0; i < DoorManager.instance.Doors.Count; i++)
            {
                Vector3 Local = DoorManager.instance.Doors[i].OBJ.localPosition;
                DoorManager.instance.Doors[i].OBJ.localPosition = new Vector3(Local.x, DoorManager.instance.Doors[i].MinMax.x, Local.z);
                SetGameFloat(DoorNames[i], DoorManager.instance.Doors[i].OBJ.localPosition.y);
            }

            if(InGameManager.instance.ShouldDebug)
                Debug.Log("Initialized room and reset or created all stats");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (DebugScript == true)
            Debug.Log("a new player joined");

        base.OnPlayerEnteredRoom(newPlayer);
    }
    
    public void LocalTakeDamage(int Damage)
    {
        if (!CanRecieveDamage)
        {
            Debug.Log("Should have felt: " + Damage + " Damage");
            return;
        }
            
        if (DebugScript == true)
            Debug.Log("TakeDamage: " + Damage);
        int BeforeHealth = GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer);
        int NewHealth = BeforeHealth - Damage > 0 ? BeforeHealth - Damage : 0;
        OnTakeDamage(Damage);
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("TakeDamage", RpcTarget.All);
        SetPlayerInt(PlayerHealth, NewHealth, PhotonNetwork.LocalPlayer);
        if(NewHealth == 0)
            StartCoroutine(MainPlayerDeath());
    }
    public IEnumerator MainPlayerDeath()
    {
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
        AIMagicControl.instance.MyCharacterSkin.SetActive(false);
        DoFade?.Invoke(false);
        //wait to find spawn
        yield return new WaitForSeconds(AfterDeathWait);
        AIMagicControl.instance.MyCharacterSkin.SetActive(true);
        SpawnManager.instance.SetNewPosition(Team.Spectator);
        DoFade?.Invoke(true);
    }
}