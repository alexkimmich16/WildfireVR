using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    public VFXHolder VFX;
    private FMOD.Studio.EventInstance FlameThrowerSound;
    public FMODUnity.EventReference EventRef;
    private void Start()
    {
        GetComponent<PhotonDestroy>().DestoryEvent += OnDestory;
        if (GetComponent<PhotonView>() && SoundManager.instance.CanPlaySound(SoundType.Effect))
        {
            FlameThrowerSound = FMODUnity.RuntimeManager.CreateInstance(EventRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FlameThrowerSound, GetComponent<Transform>());
            FlameThrowerSound.start();
            FlameThrowerSound.setParameterByName("Exit", 0f);
        }
            
    }
    public void OnDestory()
    {

    }
    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        VFX.SetNewState(NewState);
    }

    public void SetFlames(bool NewState)
    {
        if (GetComponent<PhotonView>())
        {
            if(SoundManager.instance.CanPlaySound(SoundType.Effect))
                FlameThrowerSound.setParameterByName("Exit", NewState ? 0f : 1f);
            GetComponent<PhotonView>().RPC("SetFlamesOnline", RpcTarget.Others, NewState);
            VFX.SetNewState(false);
        }
        else
            VFX.SetNewState(NewState);

        if (NewState == false)
            GetComponent<PhotonDestroy>().StartCountdown();
    }
}
