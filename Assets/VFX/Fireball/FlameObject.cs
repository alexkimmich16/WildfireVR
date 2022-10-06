using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    public VisualEffect FlamesVFX;
    public List<ParticleSystem> FlameParticalSystem;
    [PunRPC]
    void SetFlames(bool NewState)
    {
        if(FlamesVFX != null)
        {
            if (NewState == true)
                FlamesVFX.Play();
            else if (NewState == false)
            {
                FlamesVFX.Stop();
                GetComponent<PhotonDestroy>().StartCountdown();
            }
        }
        else if(FlameParticalSystem.Count != 0)
        {
            if (NewState == true)
                for (int i = 0; i < FlameParticalSystem.Count; i++)
                    FlameParticalSystem[i].Play();
            else if (NewState == false)
            {
                for (int i = 0; i < FlameParticalSystem.Count; i++)
                    FlameParticalSystem[i].Stop();
                GetComponent<PhotonDestroy>().StartCountdown();
            }
        }
    }

    public void SetFlamesOffline(bool NewState)
    {
        if (FlamesVFX != null)
        {
            if (NewState == true)
                FlamesVFX.Play();
            else if (NewState == false)
                FlamesVFX.Stop();
        }
        else if (FlameParticalSystem.Count != 0)
        {
            if (NewState == true)
                for (int i = 0; i < FlameParticalSystem.Count; i++)
                    FlameParticalSystem[i].Play();
            else if (NewState == false)
            {
                for (int i = 0; i < FlameParticalSystem.Count; i++)
                    FlameParticalSystem[i].Stop();
                GetComponent<PhotonDestroy>().StartCountdown();
            }
        }
    }
}
