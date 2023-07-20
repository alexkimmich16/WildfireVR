
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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

    public delegate void OnNewState(int State);
    public static event OnNewState OnGameState;
    public static event OnNewState OnDoorState;

    public bool FriendlyFireWorks(Player Other, Player Me)
    {
        if (!Exists(ID.PlayerTeam, Other) || !Exists(ID.PlayerTeam, Me))
            return false;
        
        bool IsFriendlyFire = GetPlayerTeam(Other) == GetPlayerTeam(Me);
        return (NetworkManager.instance.AllowFriendlyFire && IsFriendlyFire) || !IsFriendlyFire;    
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        //check if rage quitted in middle of game
        Team MyTeam = GetPlayerTeam(PhotonNetwork.LocalPlayer);
        if (InGameManager.instance.CurrentState == GameState.Active && MyTeam != Team.Spectator)
        {
            //other team won
            Result result = MyTeam == Team.Attack ? Result.DefenseWon : Result.AttackWon;
            Data.Secure.instance.EndGameManage(result);
        }
    }
    public bool CanRecieveDamage()
    {
        if (OverrideCanRecieveDamage)
            return true;
        if (InGameManager.instance.CurrentState != GameState.Active)
            return false;
        if (DoorManager.instance.Sequence <= DoorState.OpenOutDoor)
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
            SetGameVar(ID.GameState, GameState.Finished);
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
            Debug.Log("try connect to server");
    }
    public static bool HasConnected() { return PhotonNetwork.InRoom == true && Initialized(); }
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

        SetPlayerVar(ID.PlayerHealth, NetworkManager.instance.MaxHealth, PhotonNetwork.LocalPlayer);
        SetPlayerVar(ID.KillCount, 0, PhotonNetwork.LocalPlayer);
        SetPlayerVar(ID.DamageDone, 0, PhotonNetwork.LocalPlayer);
        SetPlayerVar(ID.Username, Steam.SteamAccess.ID, PhotonNetwork.LocalPlayer);
        //InitializeAllGameStats
        if (Initialized() == false)
        {
            SetGameVar(ID.GameState, GameState.Waiting);

            SetGameVar(ID.DoorState, (int)DoorState.Waiting);

            for (int i = 0; i < DoorManager.instance.Doors.Count; i++)
            {
                Vector3 Local = DoorManager.instance.Doors[i].OBJ.localPosition;
                DoorManager.instance.Doors[i].OBJ.localPosition = new Vector3(Local.x, DoorManager.instance.Doors[i].MinMax.x, Local.z);
            }

            if (InGameManager.instance.ShouldDebug)
                Debug.Log("Initialized room and reset or created all stats");
        }
        StartCoroutine(WaitForInitalized());
        base.OnJoinedRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (DebugScript == true)
            Debug.Log("a new player joined");

        base.OnPlayerEnteredRoom(newPlayer);
    }
    public void LocalTakeDamage(int Damage, Player AttackingPlayer)
    {
        if (CanRecieveDamage() == false)
        {
            Debug.Log("Should have felt: " + Damage + " Damage");
            return;
        }
        int CurrentHealth = (int)GetPlayerVar(ID.PlayerHealth, PhotonNetwork.LocalPlayer);
        if (CurrentHealth == 0)
            return;
            
        if (DebugScript == true)
            Debug.Log("TakeDamage: " + Damage);

        //adjust other player's total damage
        if (AttackingPlayer != null)
            SetPlayerVar(ID.DamageDone, (int)GetPlayerVar(ID.DamageDone, AttackingPlayer) + Damage, AttackingPlayer);

        
        int NewHealth = Mathf.Clamp(CurrentHealth - Damage, 0, MaxHealth);
        OnTakeDamage?.Invoke();
        //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("TakeDamage", RpcTarget.All);
        SetPlayerVar(ID.PlayerHealth, NewHealth, PhotonNetwork.LocalPlayer);
        if(NewHealth == 0)
        {
            //update attacking player kill count
            if (AttackingPlayer != null)
                SetPlayerVar(ID.KillCount, (int)GetPlayerVar(ID.KillCount, AttackingPlayer) + 1, AttackingPlayer);
            

            StartCoroutine(MainPlayerDeath());
        }
            
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        //on GameState change, change for everyone

        foreach (int key in changedProps.Keys)
        {
            Debug.Log("key:" + key + " value: " + changedProps[key]);
        }
        if (changedProps.ContainsKey(changedProps.ContainsKey(ID.GameState)))
        {
            OnGameState?.Invoke((int)((GameState)changedProps[ID.GameState]));
            Debug.Log("call: " + ((GameState)changedProps[ID.GameState]).ToString());
        }
            

        //on DoorState change, change for everyone
        if (changedProps.ContainsKey(changedProps.ContainsKey(ID.DoorState)))
        {
            OnDoorState?.Invoke((int)(DoorState)changedProps[ID.DoorState]);
            Debug.Log("call2");
        }
            

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