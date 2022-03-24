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
    ///public List<Vector3> LeftLocalPos;
    ///public List<Vector3> RightLocalPos;
    ///public List<Vector3> LeftRotation;
    ///public List<Vector3> RightRotation;

    public AnimationCurves LeftLocal;
    public Vector3 GetLeftLocal(float Num)
    {
        return new Vector3(LeftLocal.X.Evaluate(Num), LeftLocal.Y.Evaluate(Num), LeftLocal.Z.Evaluate(Num));
    }


    public AnimationCurves RightLocal;

    public Vector3 GetRightLocal(float Num)
    {
        return new Vector3(RightLocal.X.Evaluate(Num), RightLocal.Y.Evaluate(Num), RightLocal.Z.Evaluate(Num));
    }
    

    public AnimationCurves LeftRot;
    public Vector3 GetLeftRot(float Num)
    {
        return new Vector3(LeftRot.X.Evaluate(Num), LeftRot.Y.Evaluate(Num), LeftRot.Z.Evaluate(Num));
    }

    public AnimationCurves RightRot;
    public Vector3 GetRightRot(float Num)
    {
        return new Vector3(RightRot.X.Evaluate(Num), RightRot.Y.Evaluate(Num), RightRot.Z.Evaluate(Num));
    }
    

    public bool Set = false;
    public int Frames;
    public List<Vector2> RotationLock;
    public Vector3 Current(AnimationCurves Curves, int Num)
    {
        return new Vector3(Curves.X.Evaluate(Num), Curves.Y.Evaluate(Num), Curves.Z.Evaluate(Num));
    }
    [System.Serializable]
    public class AnimationCurves
    {
        public AnimationCurve X;
        public AnimationCurve Y;
        public AnimationCurve Z;
    }

}
