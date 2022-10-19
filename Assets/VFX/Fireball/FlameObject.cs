using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    public VisualEffect FlamesVFX;
    public List<ParticleSystem> FlameParticalSystem;
    public bool DestoryStop = true;
    public FlameCollision FlameCol;
    public FireController FlameParent;
    private void Start()
    {
        GetComponent<PhotonDestroy>().DestoryEvent += OnDestory;
    }
    public void OnDestory()
    {
        FlameParent.ActiveFires.Remove(this);
    }
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
                if(DestoryStop)
                    GetComponent<PhotonDestroy>().StartCountdown();
            }
        }
    }

    private void Update()
    {
        if (InGameManager.instance.KeypadTesting)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SetFlamesOffline(true);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                //stop
                SetFlamesOffline(false);
            }
        }
        
    }
}
