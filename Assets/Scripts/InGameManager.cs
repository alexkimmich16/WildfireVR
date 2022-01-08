using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        public bool Taken = false;
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
    
    public static float WarmupTime = 5f;
    [HideInInspector]
    public float WarmupTimer;
    public static float FinishTime = 50f;
    private float FinishTimer;
    public static float AfterCooldownTime;

    //sides
    public static int MaxPlayers = 3;
    public static int MinPlayers = 1;

    public static bool MagicCasting = false;
    public static bool CanMove = false;

    //state + spawns
    public GameState currentState = GameState.Waiting;

    //attack first
    public List<TeamInfo> Teams = new List<TeamInfo>();
    public List<Transform> SpectatorSpawns = new List<Transform>();

    public Result result;
    public void StartGame()
    {
        CanMove = true;
        MagicCasting = true;
        //play go audio
    }
    public SpawnPoint FindSpawn()
    {
        int Side = 0;
        if (InfoSave.instance.team == Team.Attack)
            Side = 0;
        else if (InfoSave.instance.team == Team.Defense)
            Side = 1;
        if (Side != 1 && Side != 0)
            return null;

        Teams[Side].TeamSize += 1;
        for (int i = 0; i < Teams[Side].Spawns.Count; i++)
        {
            if (Teams[Side].Spawns[i].Taken == false)
            {
                Teams[Side].Spawns[i].Taken = true;
                return Teams[Side].Spawns[i];
            }
        }
        return null;
    }

    public void ProgressTime()
    {
        if(currentState == GameState.Waiting)
            if (Teams[0].TeamSize >= MinPlayers && Teams[1].TeamSize >= MinPlayers)
            {
                currentState = GameState.CountDown;

            }
            else if (Teams[0].TeamSize + Teams[1].TeamSize >= MinPlayers * 2)
            {
                //start game but adjust sides
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
            if (FinishTimer > FinishTime || Teams[0].Alive == 0 || Teams[1].Alive == 0)
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
        ProgressTime();
    }
    public Result EndResult()
    {
        if(Teams[0].Alive > Teams[1].Alive)
            return Result.AttackWon;
        else if(Teams[1].Alive > Teams[0].Alive)
            return Result.AttackWon;
        else
            return Result.Tie;
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

    public void Start()
    {
        Transform Rig = GameObject.Find("XR Rig").transform;
        SpawnPoint SpawnInfo = FindSpawn();
        Transform spawn = SpawnInfo.Point;
        Rig.transform.position = spawn.position;
    }
}
