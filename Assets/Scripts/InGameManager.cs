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
    
    //[Header("Misc")]
    [Header("Players")]
    public int MaxPlayers = 3;
    public int MinPlayers = 1;
   
    public bool BalenceTeams = false;
    public bool MagicBeforeStart = false;
    
    [Header("Time")]
    public float WarmupTime = 5f;
    public float FinishTime = 200f;
    [Header("Output")]

    public int LastTotalPlayers;

    //attack first
    public List<TeamInfo> Teams = new List<TeamInfo>();
    //public List<Transform> SpectatorSpawns = new List<Transform>();

    [Header("Debug")]
    public bool ShouldDebug;
    public GameState CurrentState;
    public List<int> Health;
    private void Start()
    {
        NetworkManager.Initialize += SpawnSequence;
    }
    #region StateEvents
    public delegate void StateEvent();
    public event StateEvent OnStartCountdown;
    public event StateEvent OnGameStart;
    public event StateEvent OnGameEnd;

    //public delegate void Outcome(Result result);
    //public event Outcome OnOutcome;


    private float Timer;

    public float ElevatorSpawnOffset;

    public bool CanMove()
    {
        return true;
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
            if(OnStartCountdown != null)
                OnStartCountdown();
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
            if(OnGameEnd != null)
                OnGameEnd();
            if (PhotonNetwork.IsMasterClient)
            {
                SetGameFloat(GameFinishTimer, 0f);
                //.Log("Outcome: " + EndResult().ToString());
                SetGameResult(EndResult());
                OnlineEventManager.FinishEvent(EndResult());
            }
            
        }
        //set odin stat
        SetGameState(state);
        //NewStateEvent((int)state);
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
            if (PhotonNetwork.IsMasterClient && Initialized())
            {
                ProgressTime();
                if (LastTotalPlayers > PhotonNetwork.PlayerList.Length && LastTotalPlayers != 0)
                {
                    RemovePlayer();
                }
                LastTotalPlayers = PhotonNetwork.PlayerList.Length;
                ReCalculateTeamSize();
            }
        }
    }

    public IEnumerator SpawnSequenceCorotine()//get appropriate team, spawn, set online
    {
        Team team = BestTeam();
        SetPlayerTeam(team, PhotonNetwork.LocalPlayer);
        yield return new WaitWhile(() => Exists(PlayerTeam, PhotonNetwork.LocalPlayer) == false); //wait for team
        SpawnPoint SpawnInfo = FindSpawn(team);
        SetNewPosition(SpawnInfo);
        ///enable view
    }
    public void RespawnToSpawnPoint()
    {
        AIMagicControl.instance.Rig.position = GetSpawnGivenIndex(GetPlayerInt(PlayerSpawn, PhotonNetwork.LocalPlayer)).position;//respawn
    }
    public void SpawnSequence()
    {
        StartCoroutine(SpawnSequenceCorotine()); 
    }
    public void SetNewPosition(SpawnPoint SpawnInfo)
    {
        Team team = GetPlayerTeam(PhotonNetwork.LocalPlayer);
        Vector3 SpawnPos = realSpawn(team);
        if(team == Team.Attack || team == Team.Defense)
            SetPlayerInt(PlayerSpawn, SpawnInfo.ListNum, PhotonNetwork.LocalPlayer);
        AIMagicControl.instance.Rig.position = SpawnPos;

        Vector3 realSpawn(Team team)
        {
            if (team == Team.Attack || team == Team.Defense)
                return new Vector3(SpawnInfo.Point.position.x, GetGameFloat(DoorNames[0]) + ElevatorSpawnOffset, SpawnInfo.Point.position.z);
            else
                return SpawnInfo.Point.position;
        }
    }
    public Transform GetSpawnGivenIndex(int Index)//get point based on online code
    {
        if (Index < Teams[0].Spawns.Count)
            return Teams[0].Spawns[Index].Point;
        else
            return Teams[1].Spawns[Index - Teams[0].Spawns.Count].Point;
    }
    #region SequenceManage


    public void RestartGame()
    {
        Debug.Log("re");
        if (GetGameState() != GameState.Finished)
            return;
        Debug.Log("start");
        OnlineEventManager.RestartEvent(); //tell all to restart, master will reset stats
    }
    #endregion
    #region Info
    public Result EndResult()
    {
        int AttackTeamAlive = TotalAlive(Team.Attack);
        int DefenseTeamAlive = TotalAlive(Team.Defense);
        if(ShouldDebug)
            Debug.Log("Def: " + DefenseTeamAlive + "Att: " + AttackTeamAlive);
        if (AttackTeamAlive > DefenseTeamAlive)
            return Result.AttackWon;
        else if (DefenseTeamAlive > AttackTeamAlive)
            return Result.DefenseWon;
        else
            return Result.UnDefined;
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
    public Team BestTeam()
    {
        if(GetGameState() != GameState.Waiting)
            return Team.Spectator;

        int AttCount = SideCount(Team.Attack);
        int DefCount = SideCount(Team.Defense);
        if (AttCount == MaxPlayers && DefCount == MaxPlayers)
            return Team.Spectator;
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
        if (team == Team.Spectator)
            return Teams[Side].Spawns[Random.Range(0, Teams[Side].Spawns.Count)];
        for (int i = 0; i < Teams[Side].Spawns.Count; i++)
        {
            string SpawnString;
            if (team == Team.Attack)
                SpawnString = AttackSpawns[i];
            else
                SpawnString = DefenseSpawns[i];
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
        /*
        if (Initialized() == false || PhotonNetwork.CurrentRoom == null)
            return false;
        */
        if (Exists(PlayerTeam, PhotonNetwork.LocalPlayer) == false)
            return false;
        if (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Spectator)
            return false;
        GameState state = GetGameState();
        if (state == GameState.Active)
            return true;
        else if (state == GameState.Waiting)
            return MagicBeforeStart;
        else
            return false;
    }
    public int TotalAlive(Team team)
    {
        int AliveNum = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(ShouldDebug)
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
    /*
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
        int OldSpawnNum = GetPlayerInt(PlayerSpawn, PhotonNetwork.LocalPlayer);
        if (OldSpawnNum > 2)
            OldSpawnNum -= 3;

        string FinalOldSpawn = OldTeamName + OldSpawnNum;
        //Debug.Log(FinalOldSpawn);
        SetGameBool(FinalOldSpawn, false);

        SpawnPoint SpawnInfo = FindSpawn(NewTeam);
        SetNewPosition(SpawnInfo);

        ReCalculateTeamSize();
    }
    */
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
    UnDefined = 2,
}
