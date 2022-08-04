using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    public VisualEffect Flames;
    [PunRPC]
    void SetFlames(bool NewState, float Speed)
    {
        Flames.playRate = Speed;
        if (NewState == true)
        {
            Flames.Play();
        }
        else if(NewState == false)
        {
            Flames.Stop();
        }
    }
}
