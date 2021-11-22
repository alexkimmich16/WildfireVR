using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionShip
{
    Down = 0,
    Forward = 1,
    Back = 2,
    Left = 3,
    Right = 4,
    Up = 5,
}

[CreateAssetMenu(fileName = "FinalMove", menuName = "FinalMove")]
public class FinalMovement : MonoBehaviour
{
    public Movements MoveType;

    public float TotalTime;
    public float Interval;

    public List<Vector3> LeftWorldPos = new List<Vector3>();
    public List<Vector3> LeftLocalPos = new List<Vector3>();
    public List<Vector3> LeftDifferencePos = new List<Vector3>();

    public List<Vector3> RightWorldPos;
    public List<Vector3> RightLocalPos;
    public List<Vector3> RightDifferencePos;

    public bool Set = false;
}
