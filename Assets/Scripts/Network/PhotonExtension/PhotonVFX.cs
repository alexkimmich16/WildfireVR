using UnityEngine;
using Photon.Pun;
public class PhotonVFX : MonoBehaviour
{
    public delegate void SetVFXCallback(bool State);
    public event SetVFXCallback SetVFX;
    public VFXHolder VFX;
    [PunRPC]
    public void SetOnlineVFX(bool State)
    {
        VFX.SetNewState(State);
        SetVFX?.Invoke(State);
    }
}
