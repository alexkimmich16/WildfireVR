
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

    #endregion
    public int MaxHealth = 100;
    public bool DebugScript = false;

    public delegate void initializeEvent();
    public static event initializeEvent OnInitialized;
    public static event initializeEvent OnDeath;
    public static event initializeEvent OnTakeDamage;

    public delegate void Fade(bool In);
    public static event Fade DoFade;

    public float AfterDeathWait;

    public bool OverrideCanRecieveDamage = false;

    public bool AllowFriendlyFire;

    public Transform playerList;
    public bool FriendlyFireWorks(Player Other, Player Me)
    {
        if (!Exists(PlayerTeam, Other) || !Exists(PlayerTeam, Me))
            return false;
        
        bool IsFriendlyFire = GetPlayerTeam(Other) == GetPlayerTeam(Me);
        return (NetworkManager.instance.AllowFriendlyFire && IsFriendlyFire) || !IsFriendlyFire;    
    }

    public bool CanRecieveDamage()
    {
        if (OverrideCanRecieveDamage)
            return true;
        if (InGameManager.instance.CurrentState != GameState.Active)
            return false;
        if (DoorManager.instance.Sequence <= SequenceState.OpenOutDoor)
            return false;
        return true;
    }

    public List<GameObject> GetPlayers()
    {
        List<GameObject> Players = new List<GameObject>();
        for (int i = 0; i < playerList.childCount; ++i)
            Players.Add(playerList.GetChild(i).gameObject);
        return Players;
    }
    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!PhotonNetwork.IsMasterClient)
            return;
        //if able to simpily reset
        if (InGameManager.instance.CurrentState == GameState.Warmup && !InGameManager.instance.AbleToStartGame())
        {
            //if alters game 
            InGameManager.instance.CancelStartup();
        }
        
        if (InGameManager.instance.ShouldEnd())
        {
            OnlineEventManager.NewState(GameState.Finished);
        }
        
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

        SetPlayerInt(PlayerHealth, NetworkManager.instance.MaxHealth, PhotonNetwork.LocalPlayer);
        if (Initialized() == false)
        {
            InitializeAllGameStats();
        }
        StartCoroutine(WaitForInitalized());
        base.OnJoinedRoom();
        void InitializeAllGameStats()
        {
            SetGameState(GameState.Waiting);

            SetGameInt(DoorState, (int)SequenceState.Waiting);
            
            for (int i = 0; i < DoorManager.instance.Doors.Count; i++)
            {
                Vector3 Local = DoorManager.instance.Doors[i].OBJ.localPosition;
                DoorManager.instance.Doors[i].OBJ.localPosition = new Vector3(Local.x, DoorManager.instance.Doors[i].MinMax.x, Local.z);
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
        if (CanRecieveDamage() == false)
        {
            Debug.Log("Should have felt: " + Damage + " Damage");
            return;
        }
        if (GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer) == 0)
            return;
            
        if (DebugScript == true)
            Debug.Log("TakeDamage: " + Damage);
        int BeforeHealth = GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer);
        int NewHealth = Mathf.Clamp(BeforeHealth - Damage, 0, MaxHealth);
        OnTakeDamage?.Invoke();
        //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("TakeDamage", RpcTarget.All);
        SetPlayerInt(PlayerHealth, NewHealth, PhotonNetwork.LocalPlayer);
        if(NewHealth == 0)
            StartCoroutine(MainPlayerDeath());
    }
    public IEnumerator MainPlayerDeath()
    {
        OnDeath?.Invoke();
        
        AIMagicControl.instance.MyCharacterSkin.SetActive(false);
        DoFade?.Invoke(false);
        //wait to find spawn
        yield return new WaitForSeconds(AfterDeathWait);
        AIMagicControl.instance.MyCharacterSkin.SetActive(true);
        SpawnManager.instance.SetNewPosition(Team.Spectator);
        DoFade?.Invoke(true);
    }
}