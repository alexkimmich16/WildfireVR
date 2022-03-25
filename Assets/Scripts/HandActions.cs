using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static Odin.MagicHelp;

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
    [HideInInspector]
    public bool Playing = false;
    [HideInInspector]
    public Vector2 Direction;
    //private float Speed;

    public bool Test;

    public Collider MyCollider;
    [HideInInspector]
    public List<bool> Around = new List<bool>();

    public SkinnedMeshRenderer meshRenderer;
    public bool SettingStats = false;

    public static int PastFrameCount = 20;
    [HideInInspector]
    public List<Vector3> PastFrames = new List<Vector3>();
   // public Vector3 MyEuler;
    public Vector3 Child;
    //public Vector3 BeforeLocal;
    public Vector3 CamLocal;
    public Vector3 LocalRotation;

    private float SpellCheckTimer;

    public void WaitFrames()
    {
        for (int i = 0; i < HM.Spells.Count; i++)
        {
            //max frames and trigger
            // if not trigger after all frames, stop
            SpellType type = HM.Spells[i].Type;
            int TypeNum = (int)type;
            int Current = HM.Spells[i].Controllers[(int)side].Current;
            FinalMovement info = HandMagic.instance.Spells[i].FinalInfo;
            if (info.Frames == Current)
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
        for (int i = 0; i < HandMagic.instance.Spells.Count; i++)
        {
            if (HM.Spells[i].Active == true)
            {
                if (HM.Spells[i].Type == SpellType.Individual)
                {
                    FinalMovement info = HandMagic.instance.Spells[i].FinalInfo;
                    int Current = HM.Spells[i].Controllers[SideNum].Current;
                    
                    if (info.Frames != Current && info.Frames > 1)
                    {
                        //Debug.Log("Rotation: " + RotationWorks(LocalRotation) + "  Distance: " + DistanceWorks() + "  Current: " + Current);
                        if (RotationWorks(LocalRotation) == true && DistanceWorks() == true)
                            HM.Spells[i].Controllers[SideNum].Current += 1;
                        else
                        {
                            DistanceWorks();
                            HM.Spells[i].Controllers[SideNum].Current = 0;
                        }
                           
                        //if (SideNum == 1 && i == 1)
                        // Debug.Log("current:  " + Current + "  distance:  " + distance + "   local:  " + transform.position.ToString("F3") + "   AveragePos:  " + Converted.ToString("F3"));
                    }

                    bool DistanceWorks()
                    {
                        //Debug.Log("i: " + i + "  SideNum: " + SideNum);
                        Vector3 Converted = GetLocalPosSide(SideNum, i, Current);
                        
                            
                        float distance = Vector3.Distance(Converted, transform.position);

                        if (SideNum == 1 && i == 1)
                        {
                            //Debug.Log("Converted: " + GetLocalPosSide(SideNum, i, Current) + "  Current: " + Current + "  distance: " + distance + "  transform.position: " + transform.position);
                            //Debug.Log("Converted: " + GetLocalPosSide(SideNum, i, 1) + "  Current: " + 1);
                        }
                        HM.Spells[i].Controllers[SideNum].Distance = distance;
                        
                        if (HM.Spells[i].Leanience > distance)
                            return true;
                        else
                            return false;
                    }

                    bool RotationWorks(Vector3 MyRotation)
                    {
                        Vector3 ObjectiveRotation = GetRotationSide(SideNum, i, Current);
                        if (info.RotationLock[0] != Vector2.zero)
                        {
                            //check for limit reached
                            if (MyRotation.x > HM.Spells[i].FinalInfo.RotationLock[0].x && MyRotation.x < HM.Spells[i].FinalInfo.RotationLock[0].y)
                            {
                                ObjectiveRotation.x = MyRotation.x;
                            }
                            else
                                return false;
                        }
                        if (info.RotationLock[1] != Vector2.zero)
                        {
                            //check for limit reached
                            if (MyRotation.y > HM.Spells[i].FinalInfo.RotationLock[1].x && MyRotation.y < HM.Spells[i].FinalInfo.RotationLock[1].y)
                            {
                                ObjectiveRotation.y = MyRotation.y;
                            }
                            else
                                return false;
                        }
                        if (info.RotationLock[2] != Vector2.zero)
                        {
                            //check for limit reached
                            if (MyRotation.z > HM.Spells[i].FinalInfo.RotationLock[2].x && MyRotation.z < HM.Spells[i].FinalInfo.RotationLock[2].y)
                            {
                                ObjectiveRotation.z = MyRotation.z;
                            }
                            else
                                return false;
                        }
                        Quaternion a = Quaternion.Euler(ObjectiveRotation);
                        Quaternion b = Quaternion.Euler(MyRotation);
                        float angle = Quaternion.Angle(a, b);

                        //if (side == Side.Right && i == 1)
                            //Debug.Log("Diff: " + angle + "  ControlerAngle: " + a + "  ObjectiveAngle: " + b);
                        //float AngleDiff = Quaternion.Angle(Quaternion.Euler(ObjectiveRotation), MyRotation);
                        HM.Spells[i].Controllers[SideNum].RotDifference = angle;
                        if (HM.Spells[i].RotLeanience > angle)
                            return true;
                        else
                            return false;

                        //Debug.Log("i: " + i + "  SideNum: " + SideNum);

                    }
                }
            }
            
        }
    }
    void Update()
    {
        SetRemoteStats();
        //CheckColliders();
        
        WaitFrames();
        LastFrameSave();
        SpellCheckTimer += Time.deltaTime;
        if (SpellCheckTimer > HM.SpellCheckTime)
        {
            SpellCheckTimer = 0;
            CheckAll();
        }
        Child = transform.GetChild(1).eulerAngles;
        //MyEuler = transform.eulerAngles;
       // BeforeLocal = MyEuler - Child;
        CamLocal = Camera.main.transform.eulerAngles;
        if (CamLocal.y > 270)
            CamLocal.y -= 360;
        if (Child.y > 270)
            Child.y -= 360;
        LocalRotation = new Vector3(Child.x, CamLocal.y - Child.y, Child.z);
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