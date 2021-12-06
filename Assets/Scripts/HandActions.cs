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

    #region Spike
    public void CheckSpike()
    {
        //make sure not to let go of trigger too early
        if (HandDebug.instance.DataFolders[0].FinalInfo.LeftLocalPos.Count < 1)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed");
            }
            SpikeFrame = 0;
            return;
        }
        if (TriggerPressed() == false)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed1");
            }
            return;
        }
        if (SettingStats == true)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed2");
            }
            return;
        }

        //if the spike
        if (Around[0] == true && SpikeSequenceActive == false)
        {
            SpikeSequenceActive = true;
            SpikeFrame = 0;
            //Debug.Log("started");
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

            if (HandDebug.instance.DataFolders[0].FinalInfo.RightLocalPos.Count - 1 == SpikeFrame)
            {
                if (Around[1] == true)
                {
                    //Debug.Log("DidSpike");
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
    #endregion

    public void CheckSpikeRegular()
    {
        //make sure not to let go of trigger too early
        if (HandDebug.instance.DataFolders[0].FinalInfo.LeftLocalPos.Count < 1)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed");
            }
            SpikeFrame = 0;
            return;
        }
        if (TriggerPressed() == false)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed1");
            }
            return;
        }
        if (SettingStats == true)
        {
            if (SpikeFrame > 0)
            {
                //Debug.Log("failed2");
            }
            return;
        }

        //if the spike
        if (Around[0] == true && SpikeSequenceActive == false)
        {
            SpikeSequenceActive = true;
            SpikeFrame = 0;
            //Debug.Log("started");
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

            if (HandDebug.instance.DataFolders[0].FinalInfo.RightLocalPos.Count - 1 == SpikeFrame)
            {
                if (Around[1] == true)
                {
                    //Debug.Log("DidSpike");
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
        if(HandDebug.instance.EngineStats == true)
        {
            CheckSpike();
        }
        else
        {
            CheckSpikeRegular();
        }
        
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
