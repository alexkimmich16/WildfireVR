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

    public List<Vector3> LeftLocalPos;
    public List<Vector3> RightLocalPos;

    public List<Vector3> LeftRotation;
    public List<Vector3> RightRotation;

    public bool Set = false;

    public List<Vector2> RotationLock;
}
