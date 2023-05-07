using UnityEngine;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
public class LedgeDrop : MonoBehaviour
{
    public float DropAmount;
    public Transform Ledge;
    public bool Lowered;
    void Update()
    {
        if(Initialized() && Exists(PlayerTeam, PhotonNetwork.LocalPlayer))
        {
            Lowered = !IsHigh();
            Ledge.position = new Vector3(Ledge.position.x, IsHigh() ? 0f : DropAmount, Ledge.position.z);
        }
            
    }

    bool IsHigh() { return Alive(PhotonNetwork.LocalPlayer) && GetPlayerTeam(PhotonNetwork.LocalPlayer) != Team.Spectator; }
}
