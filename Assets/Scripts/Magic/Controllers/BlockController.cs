using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using Photon.Pun;


/// <summary>
/// constentobjects idea no respawn just 1 on and off
/// </summary>
public class BlockController : SpellControlClass
{
    public static BlockController instance;
    void Awake() { instance = this; }


    public List<bool> Active;

    public GameObject BlockVFXObject;
    public float FlameDistanceFromHead = 2f;

    public bool HalfBlocks;
    public bool AlwaysTrue;

    private bool LastFrameBlocking;
    public bool ProperBlock() { return Active[0] == true && Active[1] == true; }
    public bool HalfBlocking() { return Active[0] == true || Active[1] == true; }
    public bool IsBlocking() { return AlwaysTrue ? true : HalfBlocks ? HalfBlocking() : ProperBlock(); }
    public override void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level)
    {
        //Debug.Log("side: " + side + "  StartOrFinish: " + StartOrFinish);
        Active[(int)side] = StartOrFinish;
        if (IsBlocking() != LastFrameBlocking)//onchangestate
        {
            BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, IsBlocking());
        }
        LastFrameBlocking = IsBlocking();
    }
    public override void InitializeSpells()
    {
        BlockVFXObject = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(MotionState.Parry, 0), Vector3.zero, Quaternion.identity);
        BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.AllBuffered, false);
    }
    private void Update()
    {
        if (BlockVFXObject == null)
            return;
        Vector3 ForwardRot = Quaternion.Euler(new Vector3(0, AIMagicControl.instance.Cam.eulerAngles.y, 0)) * Vector3.forward;
        Vector3 Pos = AIMagicControl.instance.Cam.position + (ForwardRot * FlameDistanceFromHead);
        BlockVFXObject.transform.position = Pos;
        BlockVFXObject.transform.rotation = AIMagicControl.instance.Cam.rotation;
    }
}
