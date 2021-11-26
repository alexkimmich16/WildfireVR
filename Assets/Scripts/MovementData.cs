using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a simulated animation used to get finalmovement
/// </summary>

[CreateAssetMenu(menuName = "Container/MovementData")]
public class MovementData : ScriptableObject
{
    public Movements MoveType;

    public float Time;
    public float Interval;

    public List<Vector3> LeftLocalPos;
    public List<Vector3> LeftWorldPos;
    public List<Vector3> LeftDifferencePos;

    public List<Vector3> RightLocalPos;
    public List<Vector3> RightWorldPos;
    public List<Vector3> RightDifferencePos;

    public bool Set = false;
}