using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager instance;
    void Awake() { instance = this; }

    public float RespawnTime;
    public float FadeStartTime;

    public delegate void RespawnEvent();
    public static event RespawnEvent OnElevatorRespawn;


    public List<Transform> Spawns = new List<Transform>();
    void Start()
    {
        NetworkManager.OnInitialized += SpawnSequence;
    }
    
    public void JoinAsSpectator()
    {
        if (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Spectator && GetGameState() == GameState.Waiting)
        {
            SpawnSequence();
        }
    }
    
    public void SetNewPosition(Team team)
    {
        Vector3 SpawnPos = Spawns[(int)team].position;
        AIMagicControl.instance.Rig.position = SpawnPos;
    }
    public void RespawnToTeam()
    {
        Team team = GetPlayerTeam(PhotonNetwork.LocalPlayer);
        if (team != Team.Spectator)
            SetNewPosition(team);
    }
    public void SpawnSequence()
    {
        //Debug.Log("startcorotine");
        StartCoroutine(FirstSpawnSequence());
    }
    public IEnumerator FirstSpawnSequence()//get appropriate team, spawn, set online
    {
        Team team = InGameManager.instance.BestTeamForSpawn();
        SetPlayerTeam(team, PhotonNetwork.LocalPlayer);
        yield return new WaitWhile(() => Exists(PlayerTeam, PhotonNetwork.LocalPlayer) == false); //wait for team
        SetNewPosition(team);
        OnElevatorRespawn?.Invoke();
        //Debug.Log("spawncorotine2");
        ///enable view
    }

    //auto respawn player on team switch
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (InGameManager.instance.CurrentState != GameState.Waiting)
            return;
        
        if (targetPlayer.IsLocal && changedProps.ContainsKey(PlayerTeam))
            SetNewPosition((Team)changedProps[PlayerTeam]);
    }

}
