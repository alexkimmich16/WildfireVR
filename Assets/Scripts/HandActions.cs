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

    public bool Left;

    public ParticleSystem PS;

    public bool Playing = false;

    private bool Odd;
    
    private int ForceState;
    private float StateTimer = 0;
    private float ProgressTimer = 0;
    private Vector3 old;
    public Vector2 Direction;
    private float Speed;

    public bool Test;

    public Collider MyCollider;

    public List<bool> Around = new List<bool>();

    private bool SpikeSequenceActive = false;

    private int SpikeFrame;

    public SkinnedMeshRenderer meshRenderer;
    public bool SettingStats = false;


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
    public void CheckSpike()
    {
        //make sure not to let go of trigger too early
        if (HandDebug.instance.DataFolders[0].FinalInfo.LeftLocalPos.Count < 1)
        {
            if (SpikeFrame > 0)
            {
                Debug.Log("failed");
            }
            SpikeFrame = 0;
            return;
        }
        if (TriggerPressed() == false)
        {
            if (SpikeFrame > 0)
            {
                Debug.Log("failed1");
            }
            return;
        }
        if (SettingStats == true)
        {
            if (SpikeFrame > 0)
            {
                Debug.Log("failed2");
            }
            return;
        }
            
        //if the spike
        if (Around[0] == true && SpikeSequenceActive == false)
        {
            SpikeSequenceActive = true;
            SpikeFrame = 0;
            Debug.Log("started");
        }
        if (SpikeSequenceActive == true)
        {
            //distance
            //time
            meshRenderer.material = HM.Active;
            Vector3 pos = transform.position - HandDebug.instance.Player.position;
            //Debug.Log("pt2");

            
            float distance;
            if (Left == true)
            {
                distance = Vector3.Distance(pos, HandDebug.instance.DataFolders[0].FinalInfo.LeftLocalPos[SpikeFrame]);
            }
            else
            {
                distance = Vector3.Distance(pos, HandDebug.instance.DataFolders[0].FinalInfo.RightLocalPos[SpikeFrame]);
                //Debug.Log(SpikeFrame + " Dis:  " + distance);
            }
            
            //Debug.Log("pt3");

            if (HandDebug.instance.DataFolders[0].FinalInfo.RightLocalPos.Count -1 == SpikeFrame)
            {
                if (Around[1] == true)
                {
                    Debug.Log("DidSpike");
                    HM.StartSpike();
                    SpikeFrame = 0;
                }
                else
                {
                    SpikeFrame = 0;
                    //Debug.Log("failed");
                }
            }
            //if inbounds go to next
            //else stop
            if (HandDebug.instance.Leanience > distance)
            {
                SpikeSequenceActive = true;
                meshRenderer.material = HM.Active;
            }
            else
            {
                SpikeSequenceActive = false;
                meshRenderer.material = HM.DeActive;
            }
            SpikeFrame += 1;
        }
    }

    /*
    public void CheckForcePush()
    {
        //constant positive acceleation = CPA
        //require weightlike motion like star wars

        //eventually track rotation

        if (Trigger > HM.TriggerThreshold)
        {
            //Debug.Log();
        }

        //may or may not use this step
        //5 -  quick stop reguarless of CPA
        int S = ForceState;
        float Min = HM.ForcePush[S].MinVelocity;
        float Max = HM.ForcePush[S].MaxVelocity;

        ///get current velocity
        float CurrentVelocity = Speed;

        //check for state 0 requirements if meets requirements

        if (Left == false && Test == true)
        {
            //float StateTime = HM.ForcePush[S].Time;
            HM.ChangeText("NeededProgress:" + HM.ForcePush[S].Time.ToString(), 0);
            HM.ChangeText("CurrentProgress:" + ProgressTimer.ToString(), 1);
            HM.ChangeText("ResetTime:" + HM.ForcePush[S].AfterTimeDelay.ToString(), 2);
            HM.ChangeText("StateTime:" + StateTimer.ToString(), 3);

            HM.ChangeText("DesiredSpeed:" + Min.ToString() + " To "+ Max.ToString(), 4);
            HM.ChangeText("CurrentSpeed:" + Speed.ToString(), 5);
            
            HM.ChangeText("State:" + S.ToString(), 6);
        }
        //AfterTimeDelay
        
        //if is in delay period don't cancel or first
        if(S == 0 && ProgressTimer > 0)
        {
            StateTimer += Time.deltaTime;
        }
        else if (S != 0)
        {
            StateTimer += Time.deltaTime;
        }
        
        if (StateTimer < HM.ForcePush[S].AfterTimeDelay || Max > CurrentVelocity && CurrentVelocity > Min)
        {
            if (Max > CurrentVelocity && CurrentVelocity > Min)
            {
                ProgressTimer += Time.deltaTime;
                
                //progress to next
                if (ProgressTimer > HM.ForcePush[S].Time)
                {
                    //Debug.Log("01");
                    ForceState += 1;
                    StateTimer = 0;
                    ProgressTimer = 0;
                }

                //cast
                if (ForceState >= HM.ForcePush.Count)
                {
                    Vector3 pos = transform.position;
                    Direction.x = 0;

                    //find this somehow
                    //maybe by compairing pushing up vs horizonal left + right vs forward + backward
                    Debug.Log("Finished");
                    float ZDirection = 0;
                    HM.UseForcePush(ZDirection, pos, Direction);
                    //push and reset
                    //Debug.Log("02");
                    ForceState = 0;
                    StateTimer = 0;
                    ProgressTimer = 0;
                }

            }
        }
        else
        {
            //Debug.Log("03");
            ForceState = 0;
            StateTimer = 0;
            ProgressTimer = 0;
        }
    }
    */
    public void CheckColliders()
    {
        for(int i = 0; i < Around.Count; i++)
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
    void Update()
    {
        CheckColliders();
        if (Odd == true && Left == false)
        {
            old = transform.position;
        }
        else if (Odd == false && Left == false)
        {
            Speed = (transform.position - old).magnitude / Time.deltaTime;
            Speed = Mathf.Round(Speed * 100f) / 100f;
            Direction = transform.position - old;
            //HM.ChangeText(Speed.ToString());
        }
        Odd = !Odd;
        //CheckForcePush();
        CheckSpike();
        SetRemoteStats();
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
        SpikeFrame = 1;
    }

    public void Fly()
    {
        if (Left == true)
        {
            RB.AddForce(-transform.right * Power, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(transform.right * Power, ForceMode.Impulse);
        }
    }
}
