using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
[System.Serializable]
public class VFXHolder
{
    public VisualEffect VFXGraph;
    public List<ParticleSystem> ParticalSystem;

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
            if (NewState == true)
                for (int i = 0; i < ParticalSystem.Count; i++)
                    ParticalSystem[i].Play();
            else if (NewState == false)
            {
                for (int i = 0; i < ParticalSystem.Count; i++)
                    ParticalSystem[i].Stop();
            }
        }
    }
}
