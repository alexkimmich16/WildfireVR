using Photon.Pun;
using UnityEngine;
public class FlameObject : SpellObjectClass
{
    private FMOD.Studio.EventInstance FlameThrowerSound;
    private void Start()
    {
        GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);

        if (GetComponent<PhotonView>() && SoundManager.instance.CanPlay(SoundType.Effect))
        {
            FlameThrowerSound = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.FlamesRef);
            FlameThrowerSound.setVolume(SoundManager.instance.Volume(SoundType.Effect));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FlameThrowerSound, GetComponent<Transform>());
            FlameThrowerSound.start();
            FlameThrowerSound.setParameterByName("Exit", 0f);
        }
    }
    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        if (SoundManager.instance.CanPlay(SoundType.Effect))
            FlameThrowerSound.setParameterByName("Exit", NewState ? 0f : 1f);
        gameObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, NewState);
    }
}
