using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnManager : MonoBehaviour
{
    public static LearnManager instance;
    private void Awake() { instance = this; }
    [Header("Using")]

    public bool HeadPos;
    public bool HeadRot;
    public bool HandPos;
    public bool HandRot;
    public bool HandVel;
    public bool AdjustedHandPos;
    public bool VertDist;
    public bool HorDist;
    public bool Dist;
    public bool Angle;

    [Header("References")]
    //public BehaviorParameters parameters;
    //public LearningAgent agent;

    public HandActions Left;
    public HandActions Right;
    public Transform Cam;


    [Header("Stats")]
    public int VectorObvervationCount;
    void Start()
    {
        VectorObvervationCount = VectorObservationSpace();
    }
    public int VectorObservationSpace()
    {
        int Count = 0;
        
        if (HeadPos)
            Count += 3;
        if (HeadRot)
            Count += 3;
        if (HandPos)
            Count += 3;
        if (HandRot)
            Count += 3;
        if (HandVel)
            Count += 3;
        if (AdjustedHandPos)
            Count += 3;
        if (VertDist)
            Count += 1;
        if (HorDist)
            Count += 1;
        if (Dist)
            Count += 1;
        if (Angle)
            Count += 1;

        return Count;
    }
}
