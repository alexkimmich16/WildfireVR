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
    /*
    public bool AllPastFrames(bool Check)
    {
        if (PastFrames.Count != Capacity)
            return false;
        for (int i = 0; i < ConsecutiveFrames; i++)
            if (AtInverseIndex(i) != Check)
                return false;
        return true;
    }
    */
    public void AddToList(bool New)
    {
        if(Capacity == PastFrames.Count)
        {
            AverageVal = Average();
            RealState = FramesWork(false);
        }
            
        PastFrames.Add(New);
        if (PastFrames.Count > Capacity)
            PastFrames.RemoveAt(0);
    }

    public bool FramesWork(bool Looking)
    {
        if (trackType == TrackType.FrameAverage)
        {
            //is state true?
            //state is true when avg > thresh
            return AverageVal > FrameAverageThreshold;

            if (Looking == false)
                return AverageVal > FrameAverageThreshold;
            else
                return AverageVal < FrameAverageThreshold;
        }

        return false;
    }
}
