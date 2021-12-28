using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandActions : MonoBehaviour
{
    private HandMagic HM;
    public XRNode inputSource;

    public Rigidbody RB;

    public float Grip;
    public float Trigger;
    public float Power;

    public bool TopButtonPressed;
    public bool TopButtonTouched;
    public bool BottomButtonPressed;
    public bool BottomButtonTouched;

    public Side side;

    public bool Playing = false;
    
    public Vector2 Direction;
    //private float Speed;

    public bool Test;

    public Collider MyCollider;

    public List<bool> Around = new List<bool>();

    public SkinnedMeshRenderer meshRenderer;
    public bool SettingStats = false;

    public static int PastFrameCount = 20;

    public List<Vector3> PastFrames = new List<Vector3>();
    public void WaitFrames()
    {
        for (int i = 0; i < HM.Spells.Count; i++)
        {
            //max frames and trigger
            // if not trigger after all frames, stop
            SpellType type = HM.Spells[i].Type;
            int TypeNum = (int)type;
            int Current = HM.Spells[i].Controllers[(int)side].Current;
            FinalMovement info = HandDebug.instance.DataFolders[i].FinalInfo;
            if (info.RightLocalPos.Count == Current)
            {
                if (type == SpellType.Individual)
                {
                    if(HM.Spells[i].Controllers[(int)side].ControllerFinished[0] == false)
                        HM.Behaviour(i, 0, (int)side);
                    HM.Spells[i].Controllers[(int)side].ControllerFinished[0] = true;
                    if (TriggerPressed() == true && HM.Spells[i].Controllers[(int)side].ControllerFinished[1] == false)
                    {
                        HM.Behaviour(i, 1, (int)side);
                        HM.Spells[i].Controllers[(int)side].ControllerFinished[1] = true;
                    }
                    if (HM.Spells[i].Controllers[(int)side].ControllerFinished[1] == true && TriggerPressed() == false)
                    {
                        HM.Behaviour(i, 2, (int)side);
                        HM.Spells[i].Controllers[(int)side].ControllerFinished[0] = false;
                        HM.Spells[i].Controllers[(int)side].ControllerFinished[1] = false;
                        HM.Spells[i].Controllers[(int)side].Current = 0;
                    }
                }
            }
        }
    }

    public void CheckAll()
    {
        int SideNum = (int)side;
        for (int i = 0; i < HandDebug.instance.DataFolders.Count; i++)
        {
            if (HM.Spells[i].Type == SpellType.Individual)
            {
                FinalMovement info = HandDebug.instance.DataFolders[i].FinalInfo;
                int Current = HM.Spells[i].Controllers[SideNum].Current;
                if (info.RightLocalPos.Count != Current && info.LeftLocalPos.Count > 1)
                {
                    Vector3 UnConverted = HM.GetSide(SideNum, i, Current);
                    Vector3 Converted = HandMagic.instance.ConvertDataToPoint(UnConverted);
                    float distance = Vector3.Distance(Converted, transform.position);

                    //if (SideNum == 1 && i == 1)
                    // Debug.Log("current:  " + Current + "  distance:  " + distance + "   local:  " + transform.position.ToString("F3") + "   AveragePos:  " + Converted.ToString("F3"));

                    if (HM.Spells[i].Leanience > distance)
                        HM.Spells[i].Controllers[SideNum].Current += 1;
                    else
                        HM.Spells[i].Controllers[SideNum].Current = 0;
                }
            }
                
        }
    }
    void Update()
    {
        SetRemoteStats();
        CheckColliders();
        if(HandDebug.instance.EngineStats == true)
        {
            CheckAll();
        }
        WaitFrames();
        LastFrameSave();
    }
    public void LastFrameSave()
    {
        int Count = 0;
        for (int i = 0; i < PastFrameCount; i++)
        {
            if (PastFrames.Count < PastFrameCount)
            {
                PastFrames.Add(Vector3.zero);
                return;
            }
            Count = PastFrameCount - i - 1;
            if (Count == 0)
            {
                PastFrames[0] = transform.position;
            }
            else
            {
                PastFrames[Count] = PastFrames[Count - 1];
            }
        }
    }
    
    #region Checks
    public bool TriggerPressed()
    {
        if (Trigger > HandMagic.instance.TriggerThreshold)
        {
            return true;
        }
        return false;
    }
    public bool GripPressed()
    {
        if (Grip > HandMagic.instance.GripThreshold)
        {
            return true;
        }
        return false;
    }
    public void CheckColliders()
    {
        for (int i = 0; i < Around.Count; i++)
        {
            if (HM.AroundColliders[i].bounds.Intersects(MyCollider.bounds))
            {
                Around[i] = true;
            }
            else
            {
                Around[i] = false;
            }
        }
    }
    public void SetRemoteStats()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        device.TryGetFeatureValue(CommonUsages.trigger, out Trigger);

        device.TryGetFeatureValue(CommonUsages.grip, out Grip);

        device.TryGetFeatureValue(CommonUsages.secondaryButton, out BottomButtonPressed);
        device.TryGetFeatureValue(CommonUsages.secondaryTouch, out BottomButtonTouched);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out TopButtonPressed);
        device.TryGetFeatureValue(CommonUsages.primaryTouch, out TopButtonTouched);

        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Direction);
        if (Direction != Vector2.zero)
        {
            //Debug.Log("touchpad" + Direction);
        }
    }
    #endregion

    void Start()
    {
        HM = HandMagic.instance;
        //PS.Stop();
    }
}