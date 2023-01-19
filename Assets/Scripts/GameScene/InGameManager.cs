using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using Sirenix.OdinInspector;
#region Classes
[System.Serializable]
public class SpawnPoint
{
    public Transform Point;
    public int ListNum;
    public Team team;
}

public enum Team
{
    Attack = 0,
    Defense = 1,
    Spectator = 2,
}

#endregion
public class InGameManager : SerializedMonoBehaviour
{
    public static InGameManager instance;
    void Awake() { instance = this; }
    
    //[Header("Misc")]
    [Header("Players")]
    public int MaxPlayers = 3;
    public int MinPlayers = 1;
   
    public bool BalenceTeams = false;
    public bool MagicBeforeStart = false;
    public bool AlwaysCast = true;
    public bool AutoStart = true;

    [Header("Time")]
    public float WarmupTime = 5f;
    public float FinishTime = 200f;

    public bool ChooseAttackOnEven = true;
    [Header("Output")]

    //public List<Transform> SpectatorSpawns = new List<Transform>();

    [Header("Debug")]
    public bool ShouldDebug;
    public GameState CurrentState;
    #region StateEvents
    public delegate void StateEvent();
    public event StateEvent OnStartCountdown;
    public event StateEvent OnGameStart;
    public event StateEvent OnGameEnd;

    [ReadOnly] public float Timer;

    

    public bool CanMove()
    {
        return true;
    }
    public Team BestTeamForSpawn()
    {
        if (GetGameState() != GameState.Waiting)
            return Team.Spectator;

        int AttCount = SideCount(Team.Attack);
        int DefCount = SideCount(Team.Defense);
        if (AttCount == MaxPlayers && DefCount == MaxPlayers)
            return Team.Spectator;
        else if (AttCount == DefCount)
            return ChooseAttackOnEven ? Team.Attack : (Team)Random.Range(0, 2);
        else
            return AttCount > DefCount ? Team.Defense : Team.Attack;
    }
    public void SetNewGameState(GameState state)
    {
        //for each individually
        Debug.Log(state);
        Timer = 0f;
        if(state == GameState.Waiting)
        {
            //restart
        }
        else if (state == GameState.CountDown)
        {
            Timer = WarmupTime;
            if (OnStartCountdown != null)
                OnStartCountdown();
        }
        else if (state == GameState.Active)
        {
            if (OnGameStart != null)
                OnGameStart();
            Timer = FinishTime;
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
        CurrentState = GetGameState();
        //Debug.Log("Attack: " + Attack + "  Defense: " + Defense);

        GameState state = GetGameState();
        //Debug.Log("state: " + state);
        Timer -= (state == GameState.CountDown || (state == GameState.Active && DoorManager.instance.Sequence == SequenceState.WaitingForAllExit)) ? Time.deltaTime : 0;
        SetGameFloat(GameWarmupTimer, state == GameState.CountDown ? Timer : 0);
        SetGameFloat(GameFinishTimer, state == GameState.Active ? Timer : 0);
        if (AutoStart && SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers)
            SetNewGameState(GameState.CountDown);

        if (state == GameState.Waiting && SideCount(Team.Attack) + SideCount(Team.Defense) >= MinPlayers * 2 && BalenceTeams == true)
            ManageTeam();

        bool NextRestrictionReady = (state == GameState.CountDown) || (state == GameState.Active);
        if (NextRestrictionReady && Timer < 0)
            SetNewGameState((GameState)((int)state) + 1);
                
        else if (state == GameState.Active && (TotalAlive(Team.Attack) == 0 || TotalAlive(Team.Defense) == 0))
            SetNewGameState(GameState.Finished);
    }
    void Update()
    {
        if (!NetworkManager.HasConnected())
            return;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (!Initialized())
            return;

        ProgressTime();
    }

    public void StartGame()
    {
        if (SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers)
            SetNewGameState(GameState.CountDown);
    }
    public void CancelStartup()
    {
        if (GetGameState() == GameState.CountDown)
        {
            SetNewGameState(GameState.Waiting);
            SetGameFloat(GameWarmupTimer, 0);
        }
    }
   
    #region SequenceManage


    public void RestartGame()
    {
        //Debug.Log("pt0.1");
        if (GetGameState() != GameState.Finished)
            return;
        //Debug.Log("pt0.2");
        OnlineEventManager.instance.RestartEvent(); //tell all to restart, master will reset stats
    }
    #endregion
    #region Info
    public Result EndResult()
    {
        int AttackTeamAlive = TotalAlive(Team.Attack);
        int DefenseTeamAlive = TotalAlive(Team.Defense);
        if(ShouldDebug)
            Debug.Log("Def: " + DefenseTeamAlive + "Att: " + AttackTeamAlive);
        if (AttackTeamAlive != DefenseTeamAlive)
            return AttackTeamAlive > DefenseTeamAlive ? Result.AttackWon : Result.DefenseWon;
        return Result.UnDefined;
    }
    public int SideCount(Team team)
    {
        int Count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (Exists(PlayerTeam, PhotonNetwork.PlayerList[i]))
                if (GetPlayerTeam(PhotonNetwork.PlayerList[i]) == team)
                    Count += 1;
        return Count;
    }
    
    
    public bool CanDoMagic()
    {
        /*
        if (Initialized() == false || PhotonNetwork.CurrentRoom == null)
            return false;
        */
        if (AlwaysCast == false)
            return false;
        if (AIMagicControl.instance.AllActive() == false)
            return false;
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
            if(Exists(PlayerTeam, PhotonNetwork.PlayerList[i]) && Exists(PlayerHealth, PhotonNetwork.PlayerList[i]))
                if (Alive(PhotonNetwork.PlayerList[i]) && GetPlayerTeam(PhotonNetwork.PlayerList[i]) == team)
                    AliveNum += 1;
        }
        return AliveNum;
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
