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
    public bool AlwaysCast = true;
    public bool AutoStart = true;
    

    [Header("Time")]
    public float WarmupTime = 5f;
    public float GameTime = 200f;
    public float FinishTime = 15f;

    public bool ChooseAttackOnEven = true;

    [Header("Debug")]
    public bool ShouldDebug;
    public GameState CurrentState;
    #region StateEvents

    public delegate void StateEvent();
    public static event StateEvent OnBeginWaiting;
    public static event StateEvent OnStartCountdown;
    public static event StateEvent OnGameStart;
    public static event StateEvent OnRestart;
    public static event StateEvent OnFinish;

    public delegate void FinishEvent(Result result);
    public static event FinishEvent OnGameEnd;

    [ReadOnly] public float Timer;

    private bool SendingGameState;

    //private bool StartedCountdown = false;

    public bool ShouldEnd { get{ return TotalAlive(Team.Attack) == 0 || TotalAlive(Team.Defense) == 0 || Timer < 0;
        }}
    public bool AbleToStartGame{ get{
            bool AboveMin = SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers;
            bool CloseInSize = Mathf.Abs(SideCount(Team.Attack) - SideCount(Team.Defense)) < 2;
            return AboveMin && CloseInSize;
        }}
    public bool CanDoMagic { get{
            if (AlwaysCast == true)
                return true;
            //hands inactive/no team/dead
            if (!AIMagicControl.instance.AllActive() || !Exists(ID.PlayerTeam, PhotonNetwork.LocalPlayer) || !Alive(PhotonNetwork.LocalPlayer))
                return false;
            if (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Spectator)//spectator
                return false;

            return true;
        } }

    private void Start()
    {
        NetworkManager.OnGameState += SetNewGameState;
    }
    public Team BestTeamForSpawn()
    {
        if (CurrentState != GameState.Waiting)
            return Team.Spectator;

        if (SideCount(Team.Attack) == MaxPlayers && SideCount(Team.Defense) == MaxPlayers)
            return Team.Spectator;
        else if (SideCount(Team.Attack) == SideCount(Team.Defense))
            return ChooseAttackOnEven ? Team.Attack : (Team)Random.Range(0, 2);
        else
            return SideCount(Team.Attack) > SideCount(Team.Defense) ? Team.Defense : Team.Attack;
    }
    //called by OnlineEventmanager
    public void SetNewGameState(int stateNum)
    {
        //for each individually
        
        GameState state = (GameState)stateNum;
        //Debug.Log(state.ToString());

        //last = finished, new = waiting: means restart
        if(CurrentState == GameState.Finished && state == GameState.Waiting)
        {
            SetPlayerVar(ID.PlayerHealth, NetworkManager.instance.MaxHealth, PhotonNetwork.LocalPlayer);// reset health
            OnRestart?.Invoke();
            SpawnManager.instance.RespawnToTeam();// moveback to team
        }
        CurrentState = state;

        SendingGameState = false;
        Timer = 0f;
        if(state == GameState.Waiting)
        {
            OnBeginWaiting?.Invoke();
        }
        else if (state == GameState.Warmup)
        {
            OnStartCountdown?.Invoke();
            Timer = WarmupTime;
        }
        else if (state == GameState.Active)
        {
            OnGameStart?.Invoke();
            Timer = GameTime;
        }
        else if (state == GameState.Finished)
        {
            OnGameEnd?.Invoke(EndResult());
            Timer = FinishTime;
            if (PhotonNetwork.IsMasterClient)
            {
                //SetGameFloat(GameFinishTimer, 0f);
                SetGameVar(ID.Result, EndResult());
                //OnlineEventManager.FinishEvent(EndResult());
            }
            
        }
    }
    #endregion
    void Update()
    {
        if (!NetworkManager.HasConnected)
            return;
        if (!Initialized())
            return;
        if (SendingGameState == true)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            //if autostart try to start game
            if (AutoStart)
            {
                StartGame();
            }


            if (CurrentState == GameState.Waiting && BalenceTeams == true)
                BalanceTeams();
        }
        bool CountingDown = CurrentState == GameState.Finished || CurrentState == GameState.Warmup || (CurrentState == GameState.Active && (int)DoorManager.instance.Sequence >= (int)DoorState.WaitingForAllExit);
        if (CountingDown)
            Timer -= Time.deltaTime;

        if (!PhotonNetwork.IsMasterClient)
            return;


        if (CurrentState == GameState.Warmup && Timer < 0)
            SendState(GameState.Active);

        if (CurrentState == GameState.Active && (int)DoorManager.instance.Sequence >= (int)DoorState.WaitingForAllExit && ShouldEnd)
            SendState(GameState.Finished);

        if (CurrentState == GameState.Finished && Timer < 0)
            SendState(GameState.Waiting);

        void SendState(GameState State)
        {
            SendingGameState = true;
            SetGameVar(ID.GameState, State);
        }
    }



    #region SequenceChanges
    public void StartGame()
    {
        if (SideCount(Team.Attack) >= MinPlayers && SideCount(Team.Defense) >= MinPlayers && CurrentState == GameState.Waiting)
        {
            SetGameVar(ID.GameState, GameState.Warmup);
            //Debug.Log("start");
        }

    }
    public void CancelStartup()
    {
        if (CurrentState == GameState.Warmup)
        {
            SetGameVar(ID.GameState, GameState.Waiting);
            //SetNewGameState(GameState.Waiting);
            //SetGameFloat(GameWarmupTimer, 0);
        }
    }
    public void RestartGame()
    {
        if (CurrentState == GameState.Finished)//only restart on finish
            SetGameVar(ID.GameState, GameState.Waiting);
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
            if (Exists(ID.PlayerTeam, PhotonNetwork.PlayerList[i]))
                if (GetPlayerTeam(PhotonNetwork.PlayerList[i]) == team)
                    Count += 1;
        return Count;
    }
   
    public int TotalAlive(Team team)
    {
        int AliveNum = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (ShouldDebug)
                Debug.Log("Team: " + GetPlayerTeam(PhotonNetwork.PlayerList[i]) + "  Alive: " + Alive(PhotonNetwork.PlayerList[i]));
            if (Exists(ID.PlayerTeam, PhotonNetwork.PlayerList[i]) && Exists(ID.PlayerHealth, PhotonNetwork.PlayerList[i]))
                if (Alive(PhotonNetwork.PlayerList[i]) && GetPlayerTeam(PhotonNetwork.PlayerList[i]) == team)
                    AliveNum += 1;
        }
        return AliveNum;
        //{ return PhotonNetwork.PlayerList.Where(player => Exists(ID.PlayerTeam, player) && Exists(ID.PlayerHealth, player)).Count(player => Alive(player) && GetPlayerTeam(player) == team); }
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
        SetPlayerVar(ID.PlayerTeam, NewTeam, player);
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

public enum OutCome
{
    Win = 0,
    Loss = 1,
    UnDefined = 2,
}
