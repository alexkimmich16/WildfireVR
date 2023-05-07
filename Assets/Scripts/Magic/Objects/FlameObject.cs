using Photon.Pun;
using UnityEngine;
public class FlameObject : SpellObjectClass
{
    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);
        
    }

    public override void SetAudio(bool State)
    {
        Sound.setParameterByName("Exit", State ? 0f : 1f);
    }

    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        gameObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, NewState);
    }

}
