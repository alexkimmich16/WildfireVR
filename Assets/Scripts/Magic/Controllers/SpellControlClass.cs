using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;
using UnityEngine.XR;
using UnityEngine;
public abstract class SpellControlClass : SerializedMonoBehaviour
{
    public Spell Motion;
    public bool UseRepeatingHaptics;
    [PropertyRange(0f,1f)]public float HapticAmplitude = 0.5f;
    [HideIf("UseRepeatingHaptics")]public float HapticTime = 0.5f;
    [ShowIf("UseRepeatingHaptics")] public float HapticRefreshTime = 0.05f;
    [ShowIf("UseRepeatingHaptics")] public float HapticOverlap = 0.05f;

    public bool DebugStates;


    public abstract void InitializeSpells();
    public abstract void RecieveNewState(Side side, int State);
    public void DebugNewState(Side side, int State)
    {
        if (!DebugStates)
            return;

        Debug.Log("side: " + side.ToString() + "  State: " + State);

    }
    public void Start()
    {
        NetworkManager.OnInitialized += Initalize;
        Athena.PastFrameRecorder.disableController += ResetHaptics;

    }
    public void ResetHaptics(Side side)
    {
        if (!UseRepeatingHaptics)
            return;

        string SideMethod = side == Side.right ? "ForceHapticRight" : "ForceHapticLeft";
        CancelInvoke(SideMethod);

    }
    public void Initalize()
    {
        InitializeSpells();
        Athena.Runtime.instance.Spells[Motion].SpellEvent += RecieveNewState;
        Athena.Runtime.instance.Spells[Motion].SpellEvent += DebugNewState;
    }
    public void SetHaptics(Side side, bool State)
    {
        if (!UseRepeatingHaptics)
        {
            AIMagicControl.instance.Hands[(int)side].GetComponent<XRBaseController>().SendHapticImpulse(HapticAmplitude, HapticTime);
        }
        else
        {
            string SideMethod = side == Side.right ? "ForceHapticRight" : "ForceHapticLeft";
            if (State == true)
            {
                InvokeRepeating(SideMethod, 0f, HapticRefreshTime);
            }
            else
            {
                CancelInvoke(SideMethod);
            }
        }
    }
    public void ForceHapticRight()
    {
        AIMagicControl.instance.Hands[0].GetComponent<XRBaseController>().SendHapticImpulse(HapticAmplitude, HapticRefreshTime + HapticOverlap);
    }
    public void ForceHapticLeft()
    {
        AIMagicControl.instance.Hands[1].GetComponent<XRBaseController>().SendHapticImpulse(HapticAmplitude, HapticRefreshTime + HapticOverlap);
    }

    

    //XRBaseController
    //XRBaseController.controllers[i].SendHapticImpulse
}
