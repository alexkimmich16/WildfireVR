using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoveData", menuName = "MovementData")]
public class MovementData : ScriptableObject
{
    public Movements MoveType;

    public float Time;
    public float Interval;

    public List<Vector3> LeftWorldPos = new List<Vector3>();
    public List<Vector3> LeftLocalPos = new List<Vector3>();
    public List<Vector3> LeftDifferencePos = new List<Vector3>();

    public List<Vector3> RightWorldPos;
    public List<Vector3> RightLocalPos;
    public List<Vector3> RightDifferencePos;

    public bool Set = false;


}