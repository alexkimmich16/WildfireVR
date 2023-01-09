using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using ExitGames.Client.Photon;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    void Awake() { instance = this; }

    public List<Transform> Spawns = new List<Transform>();

    void Start()
    {
        NetworkManager.OnInitialized += SpawnSequence;
        DoorManager.OnDoorReset += RespawnToSpawnPoint;
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
    
    public void SpawnSequence()
    {
        //Debug.Log("startcorotine");
        StartCoroutine(SpawnSequenceCorotine());
    }
    public IEnumerator SpawnSequenceCorotine()//get appropriate team, spawn, set online
    {
        //Debug.Log("spawncorotine1");
        Team team = InGameManager.instance.BestTeamForSpawn();
        SetPlayerTeam(team, PhotonNetwork.LocalPlayer);
        yield return new WaitWhile(() => Exists(PlayerTeam, PhotonNetwork.LocalPlayer) == false); //wait for team
        SetNewPosition(team);
        //Debug.Log("spawncorotine2");
        ///enable view
    }
    //ElevatorOffset + DoorManager.instance.Doors[0].OBJ.position.y
    public void RespawnToSpawnPoint()
    {
        SetNewPosition(GetPlayerTeam(PhotonNetwork.LocalPlayer));
    }
}
