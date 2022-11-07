using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
[System.Serializable]
public class Door
{
    public string DoorName;
    public Transform OBJ;
    public float Speed;
    public Vector2 MinMax;
}
public class DoorManager : MonoBehaviour
{
    public static DoorManager instance;
    void Awake() { instance = this; }
    public float SlamSpeed;
    [Header("Wall")]
    public float StoneScrollSpeed;
    public Transform Wall;

    public delegate void DoorEvent(SequenceState state);
    public static event DoorEvent OnDoorChange;

    public delegate void DoorReset();
    public static event DoorReset OnDoorReset;

    [Header("States")]
    public SequenceState Sequence;

    [Header("Doors")]
    public List<Door> Doors = new List<Door>();

    public GameObject ElevatorEnterPreventor;
    public List<Collider> ElevatorColliders;
    public bool Inelevator;

    public float DropAmount;

    public float EarlyTimeDelay;
    public bool AddToTimer = true;
    public List<float> Times;

    public float Timer;
    public int Low, High;
    public bool Sent = false;
    public IEnumerator WaitUntilDoorReset()//get appropriate team, spawn, set online
    {
        yield return new WaitWhile(() => GetGameFloat(DoorNames[0]) != Doors[0].MinMax.x); //wait for team
        OnDoorReset();
        ///enable view
    }
    public void ResetDoors()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetNewSequenceState(SequenceState.Waiting);
            for (int i = 0; i < Doors.Count; i++)
            {
                Doors[i].OBJ.localPosition = new Vector3(0, Doors[i].MinMax.x, 0);
                SetGameFloat(DoorNames[i], Doors[i].OBJ.localPosition.y);
                //if(i == 0)
                    //Debug.Log()
            }
        }
        
        StartCoroutine(WaitUntilDoorReset());
    }

    public void OnRestart()
    {
        ElevatorEnterPreventor.SetActive(false);
        ResetDoors();
    }
    public void StartSequence()
    {
        if(Sequence == SequenceState.Waiting)
        {
            SetNewSequenceState(SequenceState.ElevatorMove);
            
            Doors[0].OBJ.Translate(Vector3.up * -DropAmount);
        }
            

    }
    private void Start()
    {
        InGameManager.instance.OnGameStart += StartSequence;
        OnlineEventManager.RestartEventCallback += OnRestart;
    }
    //private bool PlayerInElevator;
    public void MoveDoor(int DoorNum, out bool Completed, bool Opening)
    {
        if (Opening == true)
        {
            Doors[DoorNum].OBJ.Translate(Vector3.up * Time.deltaTime * Doors[DoorNum].Speed);
            SetGameFloat(DoorNames[DoorNum], Doors[DoorNum].OBJ.localPosition.y);
            Completed = Doors[DoorNum].OBJ.localPosition.y > Doors[DoorNum].MinMax.y;
        }
        else
        {
            Doors[DoorNum].OBJ.Translate(Vector3.down * Time.deltaTime * SlamSpeed);
            SetGameFloat(DoorNames[DoorNum], Doors[DoorNum].OBJ.localPosition.y);
            Completed = Doors[DoorNum].OBJ.localPosition.y < Doors[DoorNum].MinMax.x;
        }
        
    }
    public void SetNewSequenceState(SequenceState state)
    {
        Sent = false;
        Timer = 0f;
        if (AddToTimer) { Times.Add(Timer);  }
        if(state == SequenceState.Waiting)
            OnlineEventManager.DoorEvent((int)state);
        Sequence = state;
        SetGameInt(DoorState, (int)state);
        if (OnDoorChange != null)
            OnDoorChange(state);
    }
    public void SetPosition(int DoorNum)
    {
        Vector3 Local = Doors[DoorNum].OBJ.localPosition;
        Doors[DoorNum].OBJ.localPosition = new Vector3(Local.x, GetGameFloat(DoorNames[DoorNum]), Local.z);
    }
    public bool AllExited()
    {
        List<GameObject> AllPlayers = new List<GameObject>(ZoneController.instance.Players1);
        AllPlayers.AddRange(ZoneController.instance.Players2);
        for (int i = 0; i < ElevatorColliders.Count; i++)
            for (int j = 0; j < AllPlayers.Count; j++)
                if (ElevatorColliders[i].bounds.Contains(AllPlayers[j].transform.GetChild(0).position))
                    return false;
        return true;
    }
    bool PlayerInElevator()
    {
        for (int i = 0; i < ElevatorColliders.Count; i++)
            if (ElevatorColliders[i].bounds.Contains(Camera.main.transform.position))
                return true;
        return false;
    }

    private bool Closed1;
    private bool Closed2;
    public void UpdateElevator()
    {
        for (int i = 0; i < Doors.Count; i++)
            SetPosition(i);
    }
    void Update()
    {
        if (Initialized() == false)
            return;
        UpdateEarlySounds();
        
        void UpdateEarlySounds()
        {
            Timer += Time.deltaTime;
            if (AddToTimer)
                return;
            int state = (int)Sequence;
            //Debug.Log("State: " + state);
            if (state > Low == false || state < High == false)
                return;
            if (Times[state - 1] - Timer  < EarlyTimeDelay && Sent == false)
            {
                Sent = true;
                //Debug.Log("send: " + state);
                OnlineEventManager.DoorEvent(state + 1);
            }
        }
            
        Inelevator = PlayerInElevator();
        //GetGameInt(DoorState)
        if (GetGameInt(DoorState) == (int)SequenceState.Waiting)
        {
            ElevatorEnterPreventor.SetActive(false);
            float offset = Time.time * StoneScrollSpeed;
            Wall.GetComponent<Renderer>().materials[0].SetTextureOffset("_BaseMap", new Vector2(offset, 0));
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (Sequence == SequenceState.ElevatorMove)
            {
                MoveDoor(0, out bool Finshed, true);
                if (Finshed)
                    SetNewSequenceState(SequenceState.OpenInDoor);
            }
            if (Sequence == SequenceState.OpenInDoor)
            {
                MoveDoor(1, out bool Finshed, true);
                if (Finshed)
                    SetNewSequenceState(SequenceState.OpenOutDoor);
            }
            if (Sequence == SequenceState.OpenOutDoor)
            {
                MoveDoor(2, out bool Finshed, true);
                if (Finshed)
                    SetNewSequenceState(SequenceState.WaitingForAllExit);
            }
            if (Sequence == SequenceState.WaitingForAllExit)
            {
                if (AllExited())
                {
                    SetNewSequenceState(SequenceState.Closing);
                }
                if (PlayerInElevator() == false)
                {
                    ElevatorEnterPreventor.SetActive(true);
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
                    SetNewSequenceState(SequenceState.Finished);
                }
            }
        }
        else
            UpdateElevator();
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