using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorManager : MonoBehaviour
{
    public static DoorManager instance;
    void Awake() { instance = this; }

    [Header("Speeds")]
    public Vector2 OutDoorSpeed;
    public float InDoorSpeed;
    public float ElevatorSpeed;
    public float StoneScrollSpeed;

    [Header("States")]
    public SequenceState Sequence;

    [Header("Heights")]
    public Vector2 OutDoorMax;
    public float InDoorMax;
    public Vector2 ElevatorMax;

    [Header("References")]
    public Transform OutDoor;
    public Transform InDoor;
    public Transform Elevator;
    public Transform Wall;
    //star
    public void StartSequence()
    {
        Sequence = SequenceState.ElevatorMove;
    }
    private void Start()
    {
        Elevator.transform.localPosition = new Vector3(0, ElevatorMax.x, 0);
        OutDoor.transform.localPosition = new Vector3(0, OutDoorMax.x, 0);
        //StartSequence();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartSequence();
        }
        
        if(Sequence == SequenceState.Waiting)
        {
            float offset = Time.time * StoneScrollSpeed;
            Wall.GetComponent<Renderer>().materials[0].SetTextureOffset("_BaseMap", new Vector2(offset, 0));
            //Debug.Log(offset);
            //end when all players are in(wait for call)
        }
        if (Sequence == SequenceState.ElevatorMove)
        {
            Elevator.Translate(Vector3.up * Time.deltaTime * ElevatorSpeed);
            if(Elevator.localPosition.y > ElevatorMax.y)
            {
                Sequence = SequenceState.OpenInDoor;
            }
        }
        if (Sequence == SequenceState.OpenInDoor)
        {
            InDoor.Translate(Vector3.up * Time.deltaTime * InDoorSpeed);
            if (InDoor.localPosition.y > InDoorMax)
            {
                Sequence = SequenceState.OpenOutDoor;
            }
        }
        if (Sequence == SequenceState.OpenOutDoor)
        {
            OutDoor.Translate(Vector3.up * Time.deltaTime * OutDoorSpeed.x);
            if (OutDoor.localPosition.y > OutDoorMax.y)
            {
                Sequence = SequenceState.WaitingForAllExit;
            }
        }
        if (Sequence == SequenceState.WaitingForAllExit)
        {
            //if all exit
            if (true == true)
            {
                Sequence = SequenceState.Closing;
                //disable entry to wall
            }
        }
        if (Sequence == SequenceState.Closing)
        {
            //if all exit
            OutDoor.Translate(Vector3.down * Time.deltaTime * OutDoorSpeed.y);
            if (OutDoor.localPosition.x < OutDoorMax.x)
            {
                Sequence = SequenceState.Finished;
                //disable entry to wall
            }
        }
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