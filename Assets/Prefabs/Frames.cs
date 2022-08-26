using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Frames
{
    [Header("Input")]
    public int Capacity = 10;
    public int ConsecutiveFrames;
    
    [Header("Output")]
    public int AverageFrames;
    public float AverageVal;
    public List<bool> PastFrames;

    public float Average()
    {
        float Total = 0;
        for (int i = 0; i < AverageFrames; i++)
            Total += PastFrames[AverageFrames] ? 1 : 0;
        return Total / AverageFrames;
    }
    public bool AllPastFrames(bool Check)
    {
        if (PastFrames.Count != Capacity)
            return false;
        for (int i = 0; i < ConsecutiveFrames; i++)
            if (PastFrames[Capacity - 1] != Check)
                return false;
        return true;
    }
    public void AddToList(bool New)
    {
        AverageVal = Average();
        PastFrames.Add(New);
        if (PastFrames.Count > Capacity)
            PastFrames.RemoveAt(0);
    }
}
