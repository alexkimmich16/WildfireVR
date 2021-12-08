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

    public ParticleSystem PS;

    public bool Playing = false;
    
    private int ForceState;
    public Vector2 Direction;
    //private float Speed;

    public bool Test;

    public Collider MyCollider;

    public List<bool> Around = new List<bool>();

    public SkinnedMeshRenderer meshRenderer;
    public bool SettingStats = false;

    #region Spike
    //a motion, then press trigger to confirm, hold trigger to control it, release to activate
    //left is 0
    //supllies handmagic with info
    public void WaitFrames()
    {
        for (int i = 0; i < HM.Spells.Count; i++)
        {
            //max frames and trigger
            // if not trigger after all frames, stop
            int Current = HM.Spells[i].Controllers[(int)side].Current;
            FinalMovement info = HandDebug.instance.DataFolders[i].FinalInfo;
            if (info.RightLocalPos.Count == Current)
            {
                HM.Behaviour(i, 0, (int)side);
                HM.Spells[i].Finished[0] = true;
                if (TriggerPressed() == true && HM.Spells[i].Finished[1] == false)
                {
                    HM.Behaviour(i, 1,(int)side);
                    HM.Spells[i].Finished[1] = true;
                    //HM.StartSpike();
                }
                if (HM.Spells[i].Finished[1] == true && TriggerPressed() == false)
                {
                    HM.Behaviour(i, 2, (int)side);
                    HM.Spells[i].Finished[0] = false;
                    HM.Spells[i].Finished[1] = false;
                    HM.Spells[i].Controllers[(int)side].Current = 0;
                }
            }
            
        }
    }

    public void CheckAll()
    {
        Vector3 Localpos = transform.position - HandDebug.instance.Player.position;
        int SideNum = (int)side;
        //HM.Spells.Count
        for (int i = 0; i < HandDebug.instance.DataFolders.Count; i++)
        {
            FinalMovement info = HandDebug.instance.DataFolders[i].FinalInfo;
            int Current = HM.Spells[i].Controllers[SideNum].Current;
            
            if (info.RightLocalPos.Count != Current && info.LeftLocalPos.Count > 1)
            {
                Vector3 AveragePos;
                float distance;
                
                if (SideNum == 0)
                    AveragePos = new Vector3(info.LeftLocalPos[Current].x, info.LeftLocalPos[Current].y, info.LeftLocalPos[Current].z);
                else
                    AveragePos = new Vector3(info.RightLocalPos[Current].x, info.RightLocalPos[Current].y, info.RightLocalPos[Current].z);

                distance = Vector3.Distance(AveragePos, Localpos); 
                if (SideNum == 1)
                    //Debug.Log("current:  " + Current + "  distance:  " + distance + "   local:  " + Localpos.ToString("F3") + "   AveragePos:  " + AveragePos.ToString("F3"));

                //is it close enough, if not restart
                if (HM.Spells[i].Leanience > distance)
                    HM.Spells[i].Controllers[SideNum].Current += 1;
                else
                    HM.Spells[i].Controllers[SideNum].Current = 0;
            }
        }
    }
    #endregion

   
    void Update()
    {
        SetRemoteStats();
        CheckColliders();
        int SideNum = (int)side;
        //Speed = (transform.position - old).magnitude / Time.deltaTime;
        //Speed = Mathf.Round(Speed * 100f) / 100f;
        //Direction = transform.position - old;
        //HM.ChangeText(Speed.ToString());
        //CheckForcePush();
        if(HandDebug.instance.EngineStats == true)
        {
            CheckAll();
        }
        else
        {
            //CheckSpikeRegular();
        }
        WaitFrames();
        
    }

    

    public void Fly()
    {
        int SideNum = (int)side;
        if (SideNum == 0)
        {
            RB.AddForce(-transform.right * Power, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(transform.right * Power, ForceMode.Impulse);
        }
    }

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

    void Start()
    {
        HM = HandMagic.instance;
        PS.Stop();
    }
}
