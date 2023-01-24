using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Odin.Net;
using Photon.Pun;

public class DecalSpawner : MonoBehaviour
{
    public static DecalSpawner instance;
    private void Awake() { instance = this; }
    public static float SphereCastSize = 0.4f;
    [PunRPC]
    public void SpawnDecalAtPosition(Vector3 Pos, Quaternion Rot)
    {
        if(Physics.OverlapSphere(Pos, SphereCastSize).Length == 0)
            PhotonNetwork.Instantiate("Decal", Pos, Rot);
    }
}
