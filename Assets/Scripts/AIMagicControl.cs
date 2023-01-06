using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;
public enum SpellType
{
    Individual = 0,
    Both = 1,
}
public class AIMagicControl : MonoBehaviour
{
    void Awake() { instance = this; }
    public static AIMagicControl instance;

    public XRNode inputSource;

    //public NewControllerInfo Info;

    public List<Transform> PositionObjectives;
    public List<Transform> Hands;
    public List<Transform> Spawn;
    public Transform Rig;
    public Rigidbody PlayerRB;
    public Transform Cam;
    public Transform CamOffset;
    public Transform MyCharacterDisplay;
    public SpellContainer spells;
    

    public bool PlayerInHeadset;

    //public List<FireController> Flames;
    //public List<FireballController> Fireballs;
    public List<FireAbsorb> Absorbs;
    public List<BlockController> Blocks;

    public float DirectionLeaniency;
    public float DirectionForceThreshold;


    public bool HoldingFire()
    {
        return Absorbs[0].FireballControl == true || Absorbs[1].FireballControl == true;
    }
    public void ResetHoldingFires()
    {
        for (int i = 0; i < Absorbs.Count; i++)
            Absorbs[i].ResetHolding();
    }
    public bool IsBlocking()
    {
        return Blocks[0].Active == true && Blocks[1].Active == true;
    }
    private void Update()
    {
        SetPlayerBool(Blocking, IsBlocking(), PhotonNetwork.LocalPlayer);
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.userPresence, out PlayerInHeadset);
    }
}