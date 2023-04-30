using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
public class FlameObject : MonoBehaviour
{
    private FMOD.Studio.EventInstance FlameThrowerSound;
    private void Start()
    {
        //GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);
        //GetComponent<PhotonVFX>().VFX.SetNewState(true);
        GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);

        if (GetComponent<PhotonView>() && SoundManager.instance.CanPlaySound(SoundType.Effect))
        {
            FlameThrowerSound = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.FlamesRef);
            FlameThrowerSound.setVolume(SoundManager.instance.EffectVolume);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FlameThrowerSound, GetComponent<Transform>());
            FlameThrowerSound.start();
            FlameThrowerSound.setParameterByName("Exit", 0f);
        }
    }
    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
            FlameThrowerSound.setParameterByName("Exit", NewState ? 0f : 1f);
        gameObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, NewState);
    }


    private void OnEnable()
    {
        AmbientVFX.instance.Actives.Add(transform);
    }
    private void OnDisable()
    {
        AmbientVFX.instance.Actives.Remove(transform);
    }
}
