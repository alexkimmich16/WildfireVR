using Sirenix.OdinInspector;
using RestrictionSystem;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;
using UnityEngine.XR;
public abstract class SpellControlClass : SerializedMonoBehaviour
{
    public CurrentLearn Motion;
    public bool UseRepeatingHaptics;
    [PropertyRange(0f,1f)]public float HapticAmplitude = 0.5f;
    [HideIf("UseRepeatingHaptics")]public float HapticTime = 0.5f;
    [ShowIf("UseRepeatingHaptics")] public float HapticRefreshTime = 0.05f;
    [ShowIf("UseRepeatingHaptics")] public float HapticOverlap = 0.05f;


    public abstract void InitializeSpells();
    public abstract void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level);

    public void Start()
    {
        NetworkManager.OnInitialized += Initalize;
    }
    public void Initalize()
    {
        InitializeSpells();
        ConditionManager.instance.conditions.MotionConditions[(int)Motion - 1].OnNewState += RecieveNewState;
    }
    public void SetHaptics(Side side, bool State)
    {
        if (!UseRepeatingHaptics)
        {
            AIMagicControl.instance.Hands[(int)side].GetComponent<XRBaseController>().SendHapticImpulse(HapticAmplitude, HapticTime);
        }
        else
        {
            string Method = side == Side.right ? "ForceHapticRight" : "ForceHapticLeft";
            if (State == true)
            {
                InvokeRepeating(Method, 0f, HapticRefreshTime);
            }
            else
            {
                CancelInvoke(Method);
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
