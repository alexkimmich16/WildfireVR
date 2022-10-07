using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum TrackType
{
    FrameAverage = 0,
}
[System.Serializable]
public class Frames
{
    [Header("Input")]
    public bool CanCast;
    public TrackType trackType;
    public int Capacity = 10;
    //public int ConsecutiveFrames;
    public int AverageFrames;
    public float FrameAverageThreshold;

    [Header("Output")]
    public float AverageVal;
    public List<bool> PastFrames;
    public bool RealState;

    public bool AtInverseIndex(int Index)
    {
        int Inverse = Capacity - 1- Index;
        return PastFrames[Inverse];
    }
    public static int Asint(bool state)
    {
        if (state == true)
            return 1;
        else
            return 0;
    }
    public float Average()
    {
        float Total = 0;
        for (int i = 0; i < AverageFrames; i++)
            Total += Asint(AtInverseIndex(i));
        return Total / AverageFrames;
    }
    public void AddToList(bool New)
    {
        if (Capacity == PastFrames.Count)
        {
            AverageVal = Average();
            RealState = FramesWork();
        }
            
        PastFrames.Add(New);
        if (PastFrames.Count > Capacity)
            PastFrames.RemoveAt(0);
    }

    public bool FramesWork()
    {
        if (trackType == TrackType.FrameAverage)
        {
            //is state true?
            //state is true when avg > thresh
            return AverageVal > FrameAverageThreshold;
        }

        return false;
    }
}
