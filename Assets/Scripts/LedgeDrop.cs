using UnityEngine;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
namespace Misc
{
    public class LedgeDrop : MonoBehaviour
    {
        public float DropAmount;
        public Transform Ledge;
        public bool Lowered;
        void Update()
        {
            if (Initialized() && Exists(ID.PlayerTeam, PhotonNetwork.LocalPlayer))
            {
                Lowered = !IsHigh();
                Ledge.position = new Vector3(Ledge.position.x, IsHigh() ? 0f : DropAmount, Ledge.position.z);
            }

        }

        bool IsHigh() { return Alive(PhotonNetwork.LocalPlayer) && GetPlayerTeam(PhotonNetwork.LocalPlayer) != Team.Spectator; }
    }
}

