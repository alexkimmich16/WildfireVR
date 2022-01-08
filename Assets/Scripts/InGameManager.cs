using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    Unstarted = 0,
    Started = 1,
    WallBroken = 2,
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
    #endregion
    //timer
    public static float WarmupTime = 5f;
    private float WarmupTimer;
    public static float FinishTime = 50f;
    private float FinishTimer;
    public static float AfterCooldownTime;

    //sides
    public static int SideMax = 3;
    public int AttackTeam;
    public int DefenseTeam;

    //state + spawns
    public GameState currentState = GameState.Unstarted;
    public List<SpawnPoint> AttackSpawns = new List<SpawnPoint>();
    public List<SpawnPoint> DefenseSpawns = new List<SpawnPoint>();
    public List<Transform> SpectatorSpawns = new List<Transform>();

    public SpawnPoint FindSpawn()
    {
        if (InfoSave.instance.team == Team.Attack)
        {
            AttackTeam += 1;
            for (int i = 0; i < AttackSpawns.Count; i++)
            {
                if (AttackSpawns[i].Taken == false)
                {
                    AttackSpawns[i].Taken = true;
                    return AttackSpawns[i];
                }
            }
        }
        else if (InfoSave.instance.team == Team.Defense)
        {
            DefenseTeam += 1;
            for (int i = 0; i < DefenseSpawns.Count; i++)
            {
                if (DefenseSpawns[i].Taken == false)
                {
                    DefenseSpawns[i].Taken = true;
                    return DefenseSpawns[i];
                }
            }
        }
        return null;
    }

    public void ProgressTime()
    {
        if (WarmupTimer > WarmupTime)
        {
            currentState = GameState.Started;
        }
        else if(currentState == GameState.Unstarted)
            WarmupTimer += Time.deltaTime;

        if (FinishTimer > FinishTime)
        {
            currentState = GameState.Finished;
        }
        else if(currentState == GameState.Started)
            FinishTimer += Time.deltaTime;
    }
    void Update()
    {
        ProgressTime();
    }
    public Result EndResult()
    {
        if (FinishTimer < FinishTime)
        {
            return Result.AttackWon;
        }
        else
        {
            return Result.DefenseWon;
        }
    }
    
    public void Finish()
    {
        if (EndResult() == Result.AttackWon)
        {

        }
        else if (EndResult() == Result.DefenseWon)
        {

        }
    }

    public void Start()
    {
        Transform Rig = GameObject.Find("XR Rig").transform;
        SpawnPoint SpawnInfo = FindSpawn();
        Transform spawn = SpawnInfo.Point;
        Rig.transform.position = spawn.position;
    }
}
