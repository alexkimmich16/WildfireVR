using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
[System.Serializable]
public class VFXHolder
{
    public VisualEffect VFXGraph;
    public List<ParticleSystem> ParticalSystem;
    public VFXHolder(VisualEffect VFXGraphStat, List<ParticleSystem> ParticalSystemStat)
    {
        VFXGraph = VFXGraphStat;
        ParticalSystem = ParticalSystemStat;
    }
    public VFXHolder(ParticleSystem[] ParticalSystemStat)
    {
        List<ParticleSystem> PartipleList = new List<ParticleSystem>();
        for (int i = 0; i < ParticalSystemStat.Length; i++)
            PartipleList.Add(ParticalSystemStat[i]);
        ParticalSystem = PartipleList;
    }
    public void SetNewState(bool NewState)
    {
        if (VFXGraph != null)
        {
            if (NewState == true)
                VFXGraph.Play();
            else if (NewState == false)
                VFXGraph.Stop();
        }
        else if (ParticalSystem.Count != 0)
        {
            for (int i = 0; i < ParticalSystem.Count; i++)
                if (NewState == true)
                    ParticalSystem[i].Play();
                else
                    ParticalSystem[i].Stop();
        }
    }
}
