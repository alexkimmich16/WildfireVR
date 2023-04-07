using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using static Odin.Net;
using Photon.Pun;


/// <summary>
/// constentobjects idea no respawn just 1 on and off
/// </summary>
public class BlockController : SpellClass
{
    public static BlockController instance;
    void Awake() { instance = this; }
    public List<bool> Active;

    
    public GameObject BlockVFXObject;
    public float FlameDistanceFromHead = 2f;
    public bool Testing;

    private bool LastFrameBlocking;
    public bool IsBlocking() { return Active[0] == true && Active[1] == true; }
    public bool HalfBlocking() { return Active[0] == true || Active[1] == true; }
    public void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level)
    {
        //Debug.Log("side: " + side + "  StartOrFinish: " + StartOrFinish);
        Active[(int)side] = StartOrFinish;
        if (IsBlocking() != LastFrameBlocking)//onchangestate
        {
            BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, Testing ? HalfBlocking() : IsBlocking());
            SetPlayerBool(Blocking, IsBlocking(), PhotonNetwork.LocalPlayer);
        }
        LastFrameBlocking = IsBlocking();
    }
    private void Start()
    {
        NetworkManager.OnInitialized += InitializeBlockObject;
    }
    public void InitializeBlockObject()
    {
        ConditionManager.instance.conditions.MotionConditions[(int)CurrentLearn.FlameBlock - 1].OnNewState += RecieveNewState;
        BlockVFXObject = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.FlameBlock, 0), Vector3.zero, Quaternion.identity);
        BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
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
