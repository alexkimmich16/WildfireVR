using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using Sirenix.OdinInspector;
using System.Linq;
using Photon.Realtime;
#region Classes

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

    //private bool StartedCountdown = false;



    /// <summary>
    /// MAKE SURE OWNERSHIP DOESN"T PASS TO SOMEONE WHO JUST JOINED
    /// IF THERES NOONE ELSE RESTART THE GAME!!!
    /// </summary>
    /// <returns></returns>
    public Team BestTeamForSpawn()
    {
        if (GetGameState() != GameState.Waiting)
            return Team.Spectator;

        if (SideCount(Team.Attack) == MaxPlayers && SideCount(Team.Defense) == MaxPlayers)
            return Team.Spectator;
        else if (SideCount(Team.Attack) == SideCount(Team.Defense))
            return ChooseAttackOnEven ? Team.Attack : (Team)Random.Range(0, 2);
        else
            return SideCount(Team.Attack) > SideCount(Team.Defense) ? Team.Defense : Team.Attack;
    }
    //called by OnlineEventmanager
    public void SetNewGameState(GameState state)
    {
        //for each individually
        CurrentState = state;
        //Debug.Log("Recieve: " + state.ToString());
        //Debug.Log("newstate: " + state);
        Timer = 0f;
        if(state == GameState.Waiting)
        {
            //restart
            //StartedCountdown = false;
        }
        else if (state == GameState.Warmup)
        {
            OnStartCountdown?.Invoke();
            Timer = WarmupTime;
        }
        else if (state == GameState.Active)
        {
            OnGameStart?.Invoke();
            Timer = FinishTime;
        }
        else if (state == GameState.Finished)
        {
            OnGameEnd?.Invoke();
            if (PhotonNetwork.IsMasterClient)
            {
                //SetGameFloat(GameFinishTimer, 0f);
                SetGameResult(EndResult());
                //OnlineEventManager.FinishEvent(EndResult());
            }
            
        }
        SetGameState(state);
    }
    #endregion

    public bool AbleToStartGame()
    {
        bool AboveMin = SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers;
        bool CloseInSize = Mathf.Abs(SideCount(Team.Attack) - SideCount(Team.Defense)) < 2;
        return AboveMin && CloseInSize;
    }
    public bool ShouldEnd()//if m
    {
        bool SideGone = TotalAlive(Team.Attack) == 0 || TotalAlive(Team.Defense) == 0;
        bool TimerDone = Timer < 0 && CurrentState == GameState.Active;


        return SideGone || TimerDone;
    }
    public void ProgressTime()
    {
        if (AutoStart && SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers && CurrentState == GameState.Waiting)
            OnlineEventManager.NewState(GameState.Warmup);

        if (CurrentState == GameState.Waiting && BalenceTeams == true && PhotonNetwork.IsMasterClient)
            BalanceTeams();
        
        if(CurrentState == GameState.Warmup)
        {
            Timer -= Time.deltaTime;
            if (Timer < 0)
                OnlineEventManager.NewState(GameState.Active);
        }
 
        if (CurrentState == GameState.Active)
        {
            if ((int)DoorManager.instance.Sequence >= (int)SequenceState.WaitingForAllExit)
            {
                Timer -= Time.deltaTime;
                if (ShouldEnd())
                {
                    OnlineEventManager.NewState(GameState.Finished);
                }
            }

        }
            
    }
    void Update()
    {
        if (!NetworkManager.HasConnected())
            return;
        if (!Initialized())
            return;

        ProgressTime();
    }

    public void StartGame()
    {
        if (SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers && GetGameState() == GameState.Waiting)
            OnlineEventManager.NewState(GameState.Warmup);
    }
    public void CancelStartup()
    {
        if (GetGameState() == GameState.Warmup)
        {
            OnlineEventManager.NewState(GameState.Waiting);
            //SetNewGameState(GameState.Waiting);
            //SetGameFloat(GameWarmupTimer, 0);
        }
    }
   
    #region SequenceManage
    public void RestartGame()
    {
        if (GetGameState() == GameState.Finished)//only restart on finish
            OnlineEventManager.instance.RestartEvent();
        //tell all to restart, master will reset stats
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
        if (AlwaysCast == true)
            return true;
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
    
    //in future call on special action, button, player left, game end etc
    public void BalanceTeams()
    {
        if (SideCount(Team.Attack) + SideCount(Team.Defense) < MinPlayers * 2)
            return;

        int Difference = SideCount(Team.Attack) - SideCount(Team.Defense);
        if (Mathf.Abs(Difference) < 2)
            return;

        Team TeamToDeduct = Difference > 0 ? Team.Attack : Team.Defense;
        int TeamDifference = SideCount(Team.Attack) - SideCount(Team.Defense);

        Player player = PhotonNetwork.PlayerList.FirstOrDefault(player => GetPlayerTeam(player) == TeamToDeduct);

        //Set new player side
        Team NewTeam = GetPlayerTeam(player) == Team.Attack ? Team.Defense : Team.Attack;
        SetPlayerTeam(NewTeam, player);
    }
    #endregion

}

public enum GameState
{
    Waiting = 0,
    Warmup = 1,
    Active = 2,
    Finished = 3,
}
public enum Result
{
    AttackWon = 0,
    DefenseWon = 1,
    UnDefined = 2,
}
