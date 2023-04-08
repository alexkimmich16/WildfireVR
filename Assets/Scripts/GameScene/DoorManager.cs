using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using Sirenix.OdinInspector;
[System.Serializable]
public class Door
{
    public string DoorName;
    public Transform OBJ;
    public float Speed;
    public Vector2 MinMax;
}
public class DoorManager : SerializedMonoBehaviour
{
    public static DoorManager instance;
    void Awake() { instance = this; }
    public float SlamSpeed;
    [Header("Wall")]
    public float StoneScrollSpeed;
    public Transform Wall;

    public delegate void DoorReset();
    public static event DoorReset OnDoorReset;

    [Header("States")]
    public SequenceState Sequence;

    [Header("Doors")]
    public List<Door> Doors = new List<Door>();

    public float PushForce;
    public float BeforePushTime;
    private float PushTimer;


    public float DropAmount;

    public float Timer;
    public int Low, High;
    public bool Sent = false;

    [ReadOnly] public bool PlayersOutOfElevator;
    [ReadOnly] public bool InElevator;
    public float ElevatorSpawnOffset = -36.2f;
    public float ZOutOfDoor = 43f;

    public bool Finished;

    public IEnumerator WaitUntilDoorReset()//get appropriate team, spawn, set online
    {
        yield return new WaitWhile(() => Doors[0].OBJ.localPosition.y != Doors[0].MinMax.x); //wait for team
        OnDoorReset();
        ///enable view
    }
    public void ResetDoors()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            OnlineEventManager.DoorEvent(SequenceState.Waiting);
            for (int i = 0; i < Doors.Count; i++)
            {
                Doors[i].OBJ.localPosition = new Vector3(0, Doors[i].MinMax.x, 0);
                //SetGameFloat(DoorNames[i], Doors[i].OBJ.localPosition.y);
            }
        }
        
        StartCoroutine(WaitUntilDoorReset());
    }

    public void StartSequence()
    {
        if(Sequence == SequenceState.Waiting)
        {
            OnlineEventManager.DoorEvent(SequenceState.ElevatorMove);
            
            Doors[0].OBJ.Translate(Vector3.up * -DropAmount);
        }
    }
    private void Start()
    {
        InGameManager.instance.OnGameStart += StartSequence;
        OnlineEventManager.RestartEventCallback += ResetDoors;
        for (int i = 0; i < Doors.Count; i++)
            Doors[i].OBJ.localPosition = new Vector3(0, Doors[i].MinMax.x, 0);
    }
    //private bool PlayerInElevator;
    public void MoveDoor(int DoorNum, out bool Completed, bool Opening)
    {
        if (Opening == true)
        {
            Doors[DoorNum].OBJ.Translate(Vector3.up * Time.deltaTime * Doors[DoorNum].Speed);
            Completed = Doors[DoorNum].OBJ.localPosition.y > Doors[DoorNum].MinMax.y;
        }
        else
        {
            Doors[DoorNum].OBJ.Translate(Vector3.down * Time.deltaTime * SlamSpeed);
            Completed = Doors[DoorNum].OBJ.localPosition.y < Doors[DoorNum].MinMax.x;
        }
        
    }
    public void RecieveOnlineDoorState(SequenceState state)
    {
        Timer = 0f;

        if (state == SequenceState.Waiting)
            OnlineEventManager.DoorEvent(state);

        if(state == SequenceState.Finished)
            PushTimer = 0f;

        Sequence = state;
        Finished = false;
        SetGameInt(DoorState, (int)state);
    }
    public bool AllExitedElevator()
    {
        List<GameObject> players = NetworkManager.instance.GetPlayers();
        for (int i = 0; i < players.Count; i++)
            if (players[i].transform.position.z > ZOutOfDoor || players[i].transform.position.z < -ZOutOfDoor) // in elevator
                return false;
        return true;
    }
    bool PlayerInElevator() { return Camera.main.transform.position.z > ZOutOfDoor || Camera.main.transform.position.z < -ZOutOfDoor; }

    private bool Closed1;
    private bool Closed2;
    
    void Update()
    {
        if (Initialized() == false)
            return;
        PlayersOutOfElevator = AllExitedElevator();
        InElevator = PlayerInElevator();

        //UpdateEarlySounds();


        if (Sequence == SequenceState.Waiting)//WallChaange
        {
            float offset = Time.time * StoneScrollSpeed;
            Wall.GetComponent<Renderer>().materials[0].SetTextureOffset("_BaseMap", new Vector2(offset, 0));
        }
        if (Sequence == SequenceState.WaitingForAllExit)//Push
        {
            PushTimer += Time.deltaTime;
            if (PlayerInElevator() && PushTimer > BeforePushTime)
                Camera.main.transform.parent.parent.position += transform.forward * Time.deltaTime * PushForce * (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Defense ? 1 : -1);
        }

        //MoveElevator
        if((Sequence == SequenceState.ElevatorMove || Sequence == SequenceState.OpenInDoor || Sequence == SequenceState.OpenOutDoor) && !Finished)
        {
            MoveDoor((int)Sequence - 1, out Finished, true);
            if (Finished && PhotonNetwork.IsMasterClient)
                OnlineEventManager.DoorEvent((SequenceState)((int)Sequence + 1));
        }
        //Wait for exit
        if (Sequence == SequenceState.WaitingForAllExit)
        {
            if (AllExitedElevator())
            {
                OnlineEventManager.DoorEvent(SequenceState.Closing);
            }
        }
        if (Sequence == SequenceState.Closing)
        {
            //slam
            if (Closed1 == false)
                MoveDoor(1, out Closed1, false);
            if (Closed2 == false)
                MoveDoor(2, out Closed2, false);

            if (Closed1 == true && Closed2 == true)
            {
                Closed1 = false;
                Closed2 = false;
                if(PhotonNetwork.IsMasterClient)
                    OnlineEventManager.DoorEvent(SequenceState.Finished);
            }
        }
        /*
        void UpdateEarlySounds()
        {
            Timer += Time.deltaTime;
            if (AddToTimer)
                return;
            int state = (int)Sequence;
            //Debug.Log("State: " + state);
            if (state > Low == false || state < High == false)
                return;
            if (Times[state - 1] - Timer < EarlyTimeDelay && Sent == false)
            {
                Sent = true;
                //Debug.Log("send: " + state);
                OnlineEventManager.DoorEvent((SequenceState)(state + 1));
            }
        }
        */
    }
}
public enum SequenceState
{
    Waiting = 0,
    ElevatorMove = 1,
    OpenInDoor = 2,
    OpenOutDoor = 3,
    WaitingForAllExit = 4,
    Closing = 5,
    Finished = 6,
}