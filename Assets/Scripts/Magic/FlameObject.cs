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
        GetComponent<PhotonDestroy>().DestoryEvent += OnDestory;
        GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);
        if (GetComponent<PhotonView>() && SoundManager.instance.CanPlaySound(SoundType.Effect))
        {
            FlameThrowerSound = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.FlamesRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FlameThrowerSound, GetComponent<Transform>());
            FlameThrowerSound.start();
            FlameThrowerSound.setParameterByName("Exit", 0f);
        }
            
    }
    private void Update()
    {
        if(GetComponent<PhotonView>().IsMine && !FireController.instance.OnlineFire.Contains(gameObject))
        {
            gameObject.GetComponent<PhotonView>().RPC("SetFlamesOnline", RpcTarget.All, false);
        }
    }
    public void OnDestory()
    {

    }
    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
            FlameThrowerSound.setParameterByName("Exit", NewState ? 0f : 1f);
        gameObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, NewState);
    }
}
