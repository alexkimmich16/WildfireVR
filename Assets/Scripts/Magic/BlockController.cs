using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
using static Odin.Net;
using Photon.Pun;


/// <summary>
/// constentobjects idea no respawn just 1 on and off
/// </summary>
public class BlockController : MonoBehaviour
{
    public static BlockController instance;
    void Awake() { instance = this; }
    public List<bool> Active;

    
    public GameObject BlockVFXObject;
    public float FlameDistanceFromHead = 2f;
    public bool Testing;
    public bool IsBlocking() { return Active[0] == true && Active[1] == true; }
    public bool HalfBlocking() { return Active[0] == true || Active[1] == true; }
    public void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level)
    {
        //Debug.Log("side: " + side + "  StartOrFinish: " + StartOrFinish);
        Active[(int)side] = StartOrFinish;
        BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, Testing ? HalfBlocking() : IsBlocking());
    }
    private void Start()
    {
        ConditionManager.instance.MotionConditions[(int)CurrentLearn.FlameBlock - 1].OnNewState += RecieveNewState;
        NetworkManager.OnInitialized += InitializeBlockObject;
    }
    public void InitializeBlockObject()
    {
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

        SetPlayerBool(Blocking, IsBlocking(), PhotonNetwork.LocalPlayer);
        
    }
}
