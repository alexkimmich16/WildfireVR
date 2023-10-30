using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Waiting = 0,
    Warmup = 1,
    Active = 2,
    Finished = 3,
}
public enum Result
{
    AttackWon = 0,
    DefenseWon = 1,
    UnDefined = 2,
}
public enum OutCome
{
    Win = 0,
    Loss = 1,
    UnDefined = 2,
}

public enum Team
{
    Attack = 0,
    Defense = 1,
    Spectator = 2,
}

public enum DoorState
{
    Waiting = 0,
    ElevatorMove = 1,
    OpenInDoor = 2,
    OpenOutDoor = 3,
    WaitingForAllExit = 4,
    Closing = 5,
    Finished = 6,
}