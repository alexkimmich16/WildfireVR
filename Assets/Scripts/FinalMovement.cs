using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// includes final movement requirements
/// </summary>
/// 
[CreateAssetMenu(menuName = "Container/FinalData")]
public class FinalMovement : ScriptableObject
{
    public Movements MoveType;

    public float TotalTime;

    public List<Vector3> LeftWorldPos;
    public List<Vector3> LeftLocalPos;
    public List<Vector3> LeftDifferencePos;

    public List<Vector3> RightWorldPos;
    public List<Vector3> RightLocalPos;
    public List<Vector3> RightDifferencePos;

    public bool Set = false;
}

public enum DirectionShip
{
    Down = 0,
    Forward = 1,
    Back = 2,
    Left = 3,
    Right = 4,
    Up = 5,
}
