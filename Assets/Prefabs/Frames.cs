using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Frames
{
    public int ConsecutiveFrames;
    public List<bool> PastFrames;
    public int Capacity = 10;
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
        PastFrames.Add(New);
        if (PastFrames.Count > Capacity)
            PastFrames.RemoveAt(0);
    }
}
