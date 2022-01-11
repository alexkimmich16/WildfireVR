using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    public float WarmupTimer;
    public static float FinishTime = 50f;
    public float FinishTimer;
    public static float AfterCooldownTime;

    //sides
    public static int MaxPlayers = 3;
    public static int MinPlayers = 1;

    public static bool MagicCasting = false;
    public static bool CanMove = false;

    //state + spawns
    public GameState currentState = GameState.Waiting;

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
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TakenText, out object temp))
            {
                if (temp is bool)
                {
                    //bool activeGame = (bool)temp;
                    if ((bool)temp == false)
                    {
                        PhotonNetwork.CurrentRoom.CustomProperties[TakenText] = true;
                        return Teams[Side].Spawns[i];
                    }
                }
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
        int Attack = SideCount(Team.Attack);
        int Defense = SideCount(Team.Defense);
        if (currentState == GameState.Waiting)
        {
            
            if (Attack >= MinPlayers && Defense >= MinPlayers)
            {
                currentState = GameState.CountDown;
            }
            else if (Attack + Defense >= MinPlayers * 2 && BalenceTeams == true)
            {
                ManageTeam();
            }
        } 
        if (currentState == GameState.CountDown)
        {
            if (WarmupTimer > WarmupTime)
            {
                currentState = GameState.Active;
                WarmupTimer = 0;
                StartGame();
            }
            else
                WarmupTimer += Time.deltaTime;
        }
        if (currentState == GameState.Active)
        {
            if (FinishTimer > FinishTime || Attack == 0 || Defense == 0)
            {
                currentState = GameState.Finished;
                FinishTimer = 0;
                Finish();
            }
            else
                FinishTimer += Time.deltaTime;
        }
        //if finish logic
        
    }
    void Update()
    {
        if (PhotonNetwork.InRoom == true)
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
    }
    public void SetNewPosition(SpawnPoint SpawnInfo)
    {
        Transform spawn = SpawnInfo.Point;

        //change listnum
        Hashtable SpawnHash = new Hashtable();
        SpawnHash.Add("SpawnNum", SpawnInfo.ListNum);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SpawnHash);

        PhotonNetwork.LocalPlayer.CustomProperties["SpawnNum"] = SpawnInfo.ListNum;
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
        var OldTeamVAR = PhotonNetwork.PlayerList[PlayerNum].CustomProperties["TEAM"];
        Team oldTeam = (Team)OldTeamVAR;
        Team NewTeam;
        if (oldTeam == Team.Attack)
            NewTeam = Team.Defense;
        else
            NewTeam = Team.Attack;

        Hashtable TeamHash = new Hashtable();
        TeamHash.Add("TEAM", NewTeam);
        PhotonNetwork.PlayerList[PlayerNum].SetCustomProperties(TeamHash);

        ChangeTeamCount(oldTeam, -1);

        // get old team and uncheck old spawn bool
        string TeamName = InfoSave.instance.team.ToString();
        var OldSpawnNumVAR = PhotonNetwork.LocalPlayer.CustomProperties["SpawnNum"];
        int OldSpawnNum = (int)OldSpawnNumVAR;
        string FinalOldSpawn = TeamName + OldSpawnNum;

        Hashtable SpawnActiveHash = new Hashtable();
        SpawnActiveHash.Add(FinalOldSpawn, false);
        PhotonNetwork.CurrentRoom.SetCustomProperties(SpawnActiveHash);

        SpawnPoint SpawnInfo = FindSpawn(NewTeam);
        SetNewPosition(SpawnInfo);
    }
    public void ChangeTeamCount(Team team, int Change)
    {
        Debug.Log(team.ToString() + "  " + Change);
        Hashtable TeamCountHash = new Hashtable();
        string Name = "";
        if (team == Team.Attack)
            Name = "AttackTeam";
        else if (team == Team.Defense)
            Name = "DefenseTeam";
        int BeforeCount = SideCount(team);
        int NewCount = BeforeCount + Change;
        TeamCountHash.Add(Name, NewCount);
        PhotonNetwork.CurrentRoom.SetCustomProperties(TeamCountHash);
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
        //play go audio
    }
    public void Finish()
    {
        CanMove = false;
        MagicCasting = false;
        if (EndResult() == Result.AttackWon)
        {

        }
        else if (EndResult() == Result.DefenseWon)
        {

        }
        result = EndResult();
        //BillBoardManager.instance.OnWin(EndResult());
        //stop game
    }
    #endregion

}
