using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using ExitGames.Client.Photon;
#region Classes
[System.Serializable]
public class SpawnPoint
{
    public Transform Point;
    public int ListNum;
}

[System.Serializable]
public class TeamInfo
{
    public string Side;
    public List<SpawnPoint> Spawns = new List<SpawnPoint>();
}

#endregion
public class InGameManager : MonoBehaviour
{
    #region Singleton
    public static InGameManager instance;
    void Awake() { instance = this; }
    #endregion
    
    [Header("Misc")]
    public bool KeypadTesting;
    [Header("Players")]
    public int MaxPlayers = 3;
    public int MinPlayers = 1;
   
    public bool BalenceTeams = false;
    public bool MagicBeforeStart = false;
    
    [Header("Time")]
    public float WarmupTime = 5f;
    public float FinishTime = 200f;
    [Header("States")]
    //public bool MagicCasting = false;
    
    //public bool CanCast = false;
    [Header("Output")]

    private Transform Rig;

    public int LastTotalPlayers;

    //attack first
    public List<TeamInfo> Teams = new List<TeamInfo>();
    public List<Transform> SpectatorSpawns = new List<Transform>();

    public Result result;
    public bool StartedSpawn = false;
    public bool FoundSpawn = false;

    private float OnSpawnMagicDelay = 0;

    private bool DoneSpawnWaiting;

    [Header("Debug")]
    public bool ShouldDebug;
    public GameState CurrentState;
    public List<int> Health;

    #region StateEvents
    public delegate void StateEvent();
    public event StateEvent StartCountdown;
    public event StateEvent OnGameStart;
    public event StateEvent OnGameEnd;
    public const byte NewStateCode = 1;

    private float Timer;

    public float ElevatorSpawnOffset;
    private void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += ReceiveNewState; }
    private void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= ReceiveNewState; }

    public void ReceiveNewState(EventData photonEvent)
    {
        if (photonEvent.Code == NewStateCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int NewState = (int)data[0];
            //Debug.Log("NewState: " + NewState);
            if (NewState == 0)
            {
                //start timer
            }
            else if(NewState == 1)
            {

            }
            else if(NewState == 2)
            {

            }
            else if (NewState == 3)
            {

            }
        }
    }
    public bool CanMove()
    {
        return true;
    }
    private void NewStateEvent(int NewState)
    {
        object[] content = new object[] { NewState };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(NewStateCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public void SetNewGameState(GameState state)
    {
        //for each individually
        Timer = 0f;
        if(state == GameState.Waiting)
        {
            //restart
        }
        else if (state == GameState.CountDown)
        {
            if(StartCountdown != null)
                StartCountdown();
        }
        else if (state == GameState.Active)
        {
            if (OnGameStart != null)
                OnGameStart();
            SetGameFloat(GameWarmupTimer, 0f);
            //BillBoardManager.instance.SetChangeButton(false);
        }
        else if (state == GameState.Finished)
        {
            OnGameEnd();
            SetGameFloat(GameFinishTimer, 0f);
            if (EndResult() == Result.AttackWon)
            {

            }
            else if (EndResult() == Result.DefenseWon)
            {

            }
            Debug.Log("Outcome: " + EndResult().ToString());
            result = EndResult();
            //set board

            //BillBoardManager.instance.OnWin(EndResult());
            BillBoardManager.instance.SetResetButton(true);
            BillBoardManager.instance.SetChangeButton(false);
            //stop game
        }
        //set odin stat
        SetGameState(state);
        NewStateEvent((int)state);
    }
    #endregion
    public void ProgressTime()
    {
        if(ShouldDebug)
            CurrentState = GetGameState();
        float WarmupTimer = GetGameFloat(GameWarmupTimer);
        float FinishTimer = GetGameFloat(GameFinishTimer);
        int Attack = SideCount(Team.Attack);
        int Defense = SideCount(Team.Defense);
        //Debug.Log("Attack: " + Attack + "  Defense: " + Defense);
        
        GameState state = GetGameState();
        if (state == GameState.Waiting)
        {
            if (Attack >= MinPlayers && Defense >= MinPlayers)
                SetNewGameState(GameState.CountDown);
            else if (Attack + Defense >= MinPlayers * 2 && BalenceTeams == true)
                ManageTeam();
        }
        if (state == GameState.CountDown)
        {
            //BillBoardManager.instance.SetChangeButton(true);
            if (Timer > WarmupTime)
            {
                SetNewGameState(GameState.Active);
                SetGameFloat(GameWarmupTimer, 0);
                Timer = 0;
            }
            else
            {
                Timer += Time.deltaTime;
                SetGameFloat(GameWarmupTimer, Timer);
            }
                
        }
        if (state == GameState.Active)
        {
            //Debug.Log("Att: " + AttackAlive + "  Def: " + DefenseAlive + " FinishTimer: " + FinishTimer);
            if (Timer > FinishTime || TotalAlive(Team.Attack) == 0 || TotalAlive(Team.Defense) == 0)
            {
                SetNewGameState(GameState.Finished);
                Timer = 0;
                SetGameFloat(GameFinishTimer, 0);
            }
            else
            {
                Timer += Time.deltaTime;
                SetGameFloat(GameFinishTimer, Timer);
            }
        }
        /*
        void UpdateTimer(string TimerName)
        {
            Timer += Time.deltaTime;
            SetGameFloat(TimerName, Timer);
        }
        */
    }
    public void RemovePlayer()
    {
        Debug.Log("playerleft");
        List<int> Current = new List<int>();
        for (int i = 0; i < 2; i++)
        {
            if (GetGameBool(DefenseSpawns[i]) == true)
                Current.Add(i);
            if (GetGameBool(AttackSpawns[i]) == true)
                Current.Add(i + 3);
        }

        List<int> All = new List<int>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            All.Add(GetPlayerInt(PlayerSpawn, PhotonNetwork.LocalPlayer));

        if (Current.Count == All.Count)
            return;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int PlayerSpawnID = GetPlayerInt(PlayerSpawn, PhotonNetwork.PlayerList[i]);
            if (!Current.Contains(PlayerSpawnID))
            {
                string spawnID = GetNameWithNumber(PlayerSpawnID);
                SetGameBool(spawnID, false);
            }
        }

        ReCalculateTeamSize();
    }
    public void ReCalculateTeamSize()
    {
        int AttackCount = 0;
        int DefenseCount = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(Exists(PlayerTeam, PhotonNetwork.PlayerList[i]))
            {
                Team team = GetPlayerTeam(PhotonNetwork.PlayerList[i]);
                if (team == Team.Attack)
                    AttackCount += 1;
                else
                    DefenseCount += 1;
            }
        }
        SetGameInt(AttackTeamCount, AttackCount);
        SetGameInt(DefenseTeamCount, DefenseCount);
    }
    
    void Update()
    {
        if (NetworkManager.HasConnected())
        {
            if (KeypadTesting)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    NewStateEvent(1);
                else if (Input.GetKeyDown(KeyCode.D))
                    NewStateEvent(2);
            }
            
            if (StartedSpawn == false)
            {
                if(GetGameState() == GameState.Waiting)
                {
                    Team team = LowTeamCount();
                    SpawnPoint SpawnInfo = FindSpawn(team);
                    SetPlayerTeam(team, PhotonNetwork.LocalPlayer);
                    SetNewPosition(SpawnInfo);
                    //Debug.Log("newpos: " + SpawnInfo.ListNum);
                    StartedSpawn = true;
                }
                else
                {
                    Debug.LogError("Not In Waiting Phase");
                }
            }
            if (StartedSpawn == true)
            {
                if(OnSpawnMagicDelay < 4)
                    OnSpawnMagicDelay += Time.deltaTime;
                else if (OnSpawnMagicDelay > 4)
                    DoneSpawnWaiting = true;
            }

            if (PhotonNetwork.IsMasterClient && Initialized())
            {
                ProgressTime();
                if(LastTotalPlayers > PhotonNetwork.PlayerList.Length && LastTotalPlayers != 0)
                {
                    RemovePlayer();
                }
                LastTotalPlayers = PhotonNetwork.PlayerList.Length;
                ReCalculateTeamSize();
            }
        }
        if (StartedSpawn && Exists(PlayerSpawn, PhotonNetwork.LocalPlayer) == true && FoundSpawn == false)
        {
            FoundSpawn = true;
        }
    }
    public void InitialisePlayer()
    {
        Rig = GameObject.Find("XR Rig").transform;
        SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.LocalPlayer);
        //SetPlayerBool(PlayerAlive, true, PhotonNetwork.LocalPlayer);
    }
    
    public void SetNewPosition(SpawnPoint SpawnInfo)
    {
        Transform spawn = SpawnInfo.Point;
        float ElevatorPos = GetGameFloat(DoorNames[0]);
        Debug.Log(ElevatorPos + ElevatorSpawnOffset);
        Vector3 SpawnPos = new Vector3(spawn.position.x, ElevatorPos + ElevatorSpawnOffset, spawn.position.z);
        SetPlayerInt(PlayerSpawn, SpawnInfo.ListNum, PhotonNetwork.LocalPlayer);
        Rig.transform.position = SpawnPos;
    }
    public void ManageTeam()
    {
        /*
        int BalenceCount = SideCount(Team.Attack) - SideCount(Team.Defense);
        int ToLook = 3;
        if (BalenceCount < -1)
            ToLook = 1;
        else if (BalenceCount > 1)
            ToLook = 0;
        for (int i = 0; i < BalenceCount / 2; i++)
        {
            bool Found = false;
            while (Found == false)
            {
                int ToRemove = Random.Range(0, PhotonNetwork.PlayerList.Length);

                //GetPlayerTeam
                var teamVAR = PhotonNetwork.PlayerList[ToRemove].CustomProperties["TEAM"];
                Team team = (Team)teamVAR;
                if (ToLook == (int)team)
                {
                    Found = true;
                    ChangePlayerSide(ToRemove);
                }
            }
        }
        */
    }
    public void ChangePlayerSide(int PlayerNum)
    {
        Team oldTeam = GetPlayerTeam(PhotonNetwork.PlayerList[PlayerNum]);
        Team NewTeam;
        if (oldTeam == Team.Attack)
            NewTeam = Team.Defense;
        else
            NewTeam = Team.Attack;

        SetPlayerTeam(NewTeam, PhotonNetwork.PlayerList[PlayerNum]);

        // get old team and uncheck old spawn bool
        string OldTeamName = oldTeam.ToString();
        int OldSpawnNum = GetPlayerInt("SpawnNum", PhotonNetwork.LocalPlayer);
        if (OldSpawnNum > 2)
            OldSpawnNum -= 3;

        string FinalOldSpawn = OldTeamName + OldSpawnNum;
        //Debug.Log(FinalOldSpawn);
        SetGameBool(FinalOldSpawn, false);

        SpawnPoint SpawnInfo = FindSpawn(NewTeam);
        SetNewPosition(SpawnInfo);

        ReCalculateTeamSize();
    }
    public void RestartGame()
    {
        BillBoardManager.instance.SetResetButton(false);
        SetGameState(GameState.CountDown);

        //reset spawn stats
        for (int i = 0; i < MaxPlayers; i++)
        {
            string AttackText = "Attack" + i;
            string DefenseText = "Defense" + i;
            SetGameBool(AttackText, false);
            SetGameBool(DefenseText, false);
        }
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.PlayerList[i]);
        }
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        for (int i = 0; i < photonViews.Length; i++)
        {
            if (photonViews[i].IsMine)
            {
                //photonViews[i].RPC("FindSpotRPC", RpcTarget.All);
                photonViews[i].gameObject.GetComponent<NetworkPlayer>().RespawnAll();
                
            }
            
        }
        //set everything
        //than playerset
        //for all players in stand, assign random team, and teleport, and allow them to switch in cooldown

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //PhotonNetwork.PlayerList[i].photonView.RPC("changeColour", RpcTarget.AllBuffered, r, g, b);
        }
    }

    #region Info
    public Result EndResult()
    {
        int AttackTeamAlive = GetGameInt(AttackTeamCount);
        int DefenseTeamAlive = GetGameInt(DefenseTeamCount);
        if (AttackTeamAlive > DefenseTeamAlive)
            return Result.AttackWon;
        else if (DefenseTeamAlive > AttackTeamAlive)
            return Result.DefenseWon;
        else
            return Result.Tie;
    }
    public int SideCount(Team team)
    {
        if (team == Team.Attack)
            return GetGameInt(AttackTeamCount);
        else if (team == Team.Defense)
            return GetGameInt(DefenseTeamCount);
        Debug.LogError("SideCount Error!");
        return 0;
    }
    public Vector3 RandomSpectatorPos()
    {
        return SpectatorSpawns[Random.Range(0, SpectatorSpawns.Count - 1)].position;
    }
    public Team LowTeamCount()
    {
        int AttCount = SideCount(Team.Attack);
        int DefCount = SideCount(Team.Defense);
        if (AttCount > DefCount)
            return Team.Defense;
        else if (AttCount < DefCount)
            return Team.Attack;
        else
        {
            int Rand = Random.Range(0, 2);
            if (Rand == 0)
                return Team.Attack;
            else
                return Team.Defense;
        }

    }
    public SpawnPoint FindSpawn(Team team)
    {
        int Side = (int)team;
        for (int i = 0; i < Teams[Side].Spawns.Count; i++)
        {
            string SpawnString;
            if (team == Team.Attack)
            {
                SpawnString = AttackSpawns[i];
            }
            else
            {
                SpawnString = DefenseSpawns[i];
            }
            if (GetGameBool(SpawnString) == false)
            {
                SetGameBool(SpawnString, true);
                return Teams[Side].Spawns[i];
            }

        }
        Debug.LogError("Could not find spawn of type: " + team.ToString());
        return null;
    }
    public bool CanDoMagic()
    {
        if (Initialized() == false || PhotonNetwork.CurrentRoom == null)
            return false;
        GameState state = GetGameState(); 
        if (DoneSpawnWaiting == false)
            return false;
        if (state == GameState.Active)
            return true;
        else
            return MagicBeforeStart;
    }
    public int TotalAlive(Team team)
    {
        int AliveNum = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("Team: " + GetPlayerTeam(PhotonNetwork.PlayerList[i]) + "  Alive: " + Alive(PhotonNetwork.PlayerList[i]));
            if (Alive(PhotonNetwork.PlayerList[i]) && GetPlayerTeam(PhotonNetwork.PlayerList[i]) == team)
                AliveNum += 1;
        }
        return AliveNum;
    }
    public string GetNameWithNumber(int Index)
    {
        if (Index < 3)
            return AttackSpawns[Index];
        else
            return DefenseSpawns[Index - 3];
        //ebug.LogError("Could not Spawn With Index: " + Index);
    }
    #endregion
}

public enum GameState
{
    Waiting = 0,
    CountDown = 1,
    Active = 2,
    Finished = 3,
}
public enum Result
{
    AttackWon = 0,
    DefenseWon = 1,
    Tie = 2,
}
