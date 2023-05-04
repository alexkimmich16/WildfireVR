using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using Sirenix.OdinInspector;
using System.Linq;
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

    [Header("States")]
    public SequenceState Sequence;

    [Header("Doors")]
    public List<Door> Doors = new List<Door>();

    public float PushForce;
    public float BeforePushTime;


    public float DropAmount;

    public float Timer;
    public int Low, High;
    public bool Sent = false;

    [ReadOnly] public bool PlayersOutOfElevator;
    [ReadOnly] public bool InElevator;
    public float ElevatorSpawnOffset = -36.2f;
    public float ZOutOfDoor = 43f;

    public bool Finished;

    private bool IsCounting;

    public void ResetDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].OBJ.localPosition = new Vector3(0, Doors[i].MinMax.x, 0);
            //SetGameFloat(DoorNames[i], Doors[i].OBJ.localPosition.y);
        }
        OnlineEventManager.DoorEvent(SequenceState.Waiting);
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

        //manage stop rigidbody movement and clipping
        AIMagicControl.instance.Rig.transform.parent = state == SequenceState.ElevatorMove ? Doors[0].OBJ : null;
        if (state == SequenceState.ElevatorMove)
            AIMagicControl.instance.Rig.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
        else
            AIMagicControl.instance.Rig.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;

        //if (state == SequenceState.Waiting)
            //OnlineEventManager.DoorEvent(state);

        Sequence = state;
        Finished = false;
        SetGameInt(DoorState, (int)state);
    }
    public bool AllExitedElevator() { return NetworkManager.instance.GetPlayers().All(x => x.transform.position.z < ZOutOfDoor && x.transform.position.z > -ZOutOfDoor); }

    bool PlayerInElevator() { return Camera.main.transform.position.z > ZOutOfDoor || Camera.main.transform.position.z < -ZOutOfDoor; }

    private bool Closed1;
    private bool Closed2;
    public IEnumerator PushOutOfElevator()
    {
        yield return new WaitForSeconds(BeforePushTime);
        while (PlayerInElevator() && InGameManager.instance.CurrentState != GameState.Finished && Sequence == SequenceState.WaitingForAllExit)
        {
            Camera.main.transform.parent.parent.position += transform.forward * Time.deltaTime * PushForce * (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Defense ? 1 : -1);
            yield return new WaitForEndOfFrame();
        }
            
        IsCounting = false;
    }
    void FixedUpdate()
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
        if (Sequence == SequenceState.WaitingForAllExit && !IsCounting)
        {
            StartCoroutine(PushOutOfElevator());
            IsCounting = true;
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