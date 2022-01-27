using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static NetworkFunctionsAndInfo.Net;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

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

public class InGameManager : MonoBehaviour
{
    #region Singleton + classes
    public static InGameManager instance;
    void Awake() { instance = this; }

    [System.Serializable]
    public class SpawnPoint
    {
        public Transform Point;
        public int ListNum;
        //public bool Taken = false;
    }

    [System.Serializable]
    public class TeamInfo
    {
        public string Side;
        public int TeamSize;
        public int Alive;
        public List<SpawnPoint> Spawns = new List<SpawnPoint>();
    }
    #endregion

    //timer
    public static bool BalenceTeams = false;
    public static float WarmupTime = 5f;
    [HideInInspector]
    public static float FinishTime = 50f;

    //sides
    public static int MaxPlayers = 3;
    public static int MinPlayers = 1;

    public static bool MagicCasting = false;
    public static bool CanMove = false;

    //state + spawns
    //public GameState currentState = GameState.Waiting;

    private Transform Rig;

    //attack first
    public List<TeamInfo> Teams = new List<TeamInfo>();
    public List<Transform> SpectatorSpawns = new List<Transform>();

    public Result result;
    public SpawnPoint FindSpawn(Team team)
    {
        ChangeTeamCount(team, 1);

        int Side = (int)team;
        string TeamName = team.ToString();
        for (int i = 0; i < Teams[Side].Spawns.Count; i++)
        {
            string TakenText = TeamName + i;
            if (GetRoomBool(TakenText) == false)
            {
                SetRoomBool(TakenText, true);
                return Teams[Side].Spawns[i];
            }
            
        }
        return null;
    }
    public int SideCount(Team team)
    {
        string Name = "";
        if (team == Team.Attack)
            Name = "AttackTeam";
        else if (team == Team.Defense)
            Name = "DefenseTeam";

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(Name, out object temp))
        {
            return (int)temp;
        }
        else
        {
            return 100;
        }
    }
    public void ProgressTime()
    {
        if(Exists(GameWarmupTimer, null) == true)
        {
            float WarmupTimer = GetGameFloat("WarmupTimer");
            float FinishTimer = GetGameFloat("FinishTimer");
            int Attack = SideCount(Team.Attack);
            int Defense = SideCount(Team.Defense);
            GameState state = GetGameState();
            if (state == GameState.Waiting)
            {
                if (Attack >= MinPlayers && Defense >= MinPlayers)
                {
                    SetGameState(GameState.CountDown);
                }
                else if (Attack + Defense >= MinPlayers * 2 && BalenceTeams == true)
                {
                    ManageTeam();
                }
            }
            if (state == GameState.CountDown)
            {
                BillBoardManager.instance.SetChangeButton(true);
                if (WarmupTimer > WarmupTime)
                {
                    SetGameState(GameState.Active);
                    SetGameFloat("WarmupTimer", 0f);
                    StartGame();
                    BillBoardManager.instance.SetChangeButton(false);
                }
                else
                    SetGameFloat("WarmupTimer", WarmupTimer + Time.deltaTime);
            }
            if (state == GameState.Active)
            {
                if (FinishTimer > FinishTime || Attack == 0 || Defense == 0)
                {
                    SetGameState(GameState.Finished);
                    SetGameFloat("FinishTimer", 0f);
                    Finish();
                }
                else
                    SetGameFloat("FinishTimer", FinishTimer + Time.deltaTime);
            }
        }
        
        
    }
    void Update()
    {
        if (PhotonNetwork.InRoom == true && PhotonNetwork.IsMasterClient)
        {
            ProgressTime();
            //Debug.Log(SideCount(Team.Attack) + "  Attack");
            //Debug.Log(SideCount(Team.Defense) + "  Defense");
            
        }
    }
    public void Initialise()
    {
        Rig = GameObject.Find("XR Rig").transform;
        SpawnPoint SpawnInfo = FindSpawn(InfoSave.instance.team);
        SetNewPosition(SpawnInfo);

        SetPlayerTeam(InfoSave.instance.team, PhotonNetwork.LocalPlayer);
        SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.LocalPlayer);

        SetPlayerInt(PlayerSpawn, SpawnInfo.ListNum, PhotonNetwork.LocalPlayer);
        SetPlayerBool(PlayerAlive, true, PhotonNetwork.LocalPlayer);

        if (Exists(GameWarmupTimer, null) == false)
        {
            SetGameFloat(GameWarmupTimer, 0f);
            SetGameFloat(GameFinishTimer, 0f);
            SetGameState(GameState.Waiting);
            //Debug.Log("SetGame");
        }
        
    }
    public void SetNewPosition(SpawnPoint SpawnInfo)
    {
        Transform spawn = SpawnInfo.Point;

        //change listnum
        SetPlayerInt(PlayerSpawn, SpawnInfo.ListNum, PhotonNetwork.LocalPlayer);
        Rig.transform.position = spawn.position;
    }
    public void ManageTeam()
    {
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
                var teamVAR = PhotonNetwork.PlayerList[ToRemove].CustomProperties["TEAM"];
                Team team = (Team)teamVAR;
                if (ToLook == (int)team)
                {
                    Found = true;
                    ChangePlayerSide(ToRemove);
                }
            }
        }
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

        ChangeTeamCount(oldTeam, -1);

        // get old team and uncheck old spawn bool
        string TeamName = InfoSave.instance.team.ToString();
        var OldSpawnNumVAR = PhotonNetwork.LocalPlayer.CustomProperties["SpawnNum"];
        int OldSpawnNum = (int)OldSpawnNumVAR;
        string FinalOldSpawn = TeamName + OldSpawnNum;

        SetRoomBool(FinalOldSpawn, false);

        SpawnPoint SpawnInfo = FindSpawn(NewTeam);
        SetNewPosition(SpawnInfo);
    }
    public void ChangeTeamCount(Team team, int Change)
    {
        //Debug.Log(team.ToString() + "  " + Change);
        string Name = team.ToString();
        int NewCount = SideCount(team) + Change;
        SetRoomInt(Name, NewCount);
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
            SetRoomBool(AttackText, false);
            SetRoomBool(DefenseText, false);
        }

        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //set all alive
            SetPlayerBool(PlayerAlive, true, PhotonNetwork.PlayerList[i]);
            SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.PlayerList[i]);
        }
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        for (int i = 0; i < photonViews.Length; i++)
        {
            photonViews[i].RPC("FindSpotRPC", RpcTarget.All);
        }
        //get phototonview of other
        


        //set everything
        //than playerset
        //for all players in stand, assign random team, and teleport, and allow them to switch in cooldown

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //PhotonNetwork.PlayerList[i].photonView.RPC("changeColour", RpcTarget.AllBuffered, r, g, b);
        }
    }

    #region LessUse
    public Result EndResult()
    {
        if (Teams[0].Alive > Teams[1].Alive)
            return Result.AttackWon;
        else if (Teams[1].Alive > Teams[0].Alive)
            return Result.AttackWon;
        else
            return Result.Tie;
    }
    public void StartGame()
    {
        CanMove = true;
        MagicCasting = true;
        HandMagic.instance.EnableCubes(true);
        //play go audio
    }
    public void Finish()
    {
        CanMove = false;
        MagicCasting = false;
        HandMagic.instance.EnableCubes(false);
        if (EndResult() == Result.AttackWon)
        {

        }
        else if (EndResult() == Result.DefenseWon)
        {

        }
        result = EndResult();
        //set board

        //BillBoardManager.instance.OnWin(EndResult());
        BillBoardManager.instance.SetResetButton(true);
        BillBoardManager.instance.SetChangeButton(false);
        //stop game
    }
    #endregion


    
}
