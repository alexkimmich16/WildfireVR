using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RestrictionSystem;
public class FireManipulation : MonoBehaviour
{
    public static FireManipulation instance;
    void Awake() { instance = this; }

    public float Strictness;
    public float MinMagnitude;

    public float Difference;
    public bool ActiveManipulating;
    public Vector3 ActivePushDirection;

    public Transform AmbientVFXDirection;

    public float PushCooldown;
    private float CooldownTimer;

    public float PushForce;
    public float MaxManipulateRange;
    public bool IsManipulating()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 RightVelocity);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 LeftVelocity);
        
        Difference = Vector3.Angle(RightVelocity, LeftVelocity);
        //Vector3.Angle(RightVelocity, LeftVelocity);

        return Difference < Strictness && RightVelocity.magnitude > MinMagnitude && LeftVelocity.magnitude > MinMagnitude;
    }


    Vector3 PushDirection()
    {
        Vector3 RightVelocity = PastFrameRecorder.instance.GetControllerInfo(Side.right).HandPos - PastFrameRecorder.instance.PastFrame(Side.right).HandPos;
        Vector3 LeftVelocity = PastFrameRecorder.instance.GetControllerInfo(Side.left).HandPos - PastFrameRecorder.instance.PastFrame(Side.left).HandPos;
        //Distance / (frame2.SpawnTime - frame1.SpawnTime)
        //InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 RightVelocity);
        //InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 LeftVelocity);

        //InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 RightPos);
        //InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 LeftPos);

        return GetAngleHalfway(RightVelocity, LeftVelocity);
        //if()
    }

    // Update is called once per frame
    void Update()
    {
        if (!PastFrameRecorder.IsReady() || !AIMagicControl.instance.AllActive())
            return;
        CooldownTimer += Time.deltaTime;
        if (ActiveManipulating && !IsManipulating())
        {
            AmbientParticles.AmbientVFX.instance.Actives.Remove(AmbientVFXDirection);
        }
        else if(!ActiveManipulating && IsManipulating())
        {
            AmbientParticles.AmbientVFX.instance.Actives.Add(AmbientVFXDirection);
        }

        ActiveManipulating = IsManipulating();
        ActivePushDirection = PushDirection();
        for (int i = 0; i < 2; i++)
        {
            Vector3 Direction = (PastFrameRecorder.instance.GetControllerInfo((Side)i).HandPos - PastFrameRecorder.instance.PastFrame((Side)i).HandPos).normalized * 100f;
            //Debug.DrawRay(AIMagicControl.instance.Hands[i].position, Direction);
        }
        AmbientVFXDirection.forward = (PastFrameRecorder.instance.GetControllerInfo(Side.right).HandPos - PastFrameRecorder.instance.PastFrame(Side.right).HandPos).normalized;
        AmbientVFXDirection.position = (AIMagicControl.instance.Hands[0].position + AIMagicControl.instance.Hands[1].position) / 2f;

        //PushFireOnlineEvent
        if (CooldownTimer > PushCooldown && ActiveManipulating == true)
        {
            OnlineEventManager.PushFireOnlineEvent(Camera.main.transform.position, AmbientVFXDirection.forward);
            CooldownTimer = 0f;
        }
        //Debug.DrawRay(AmbientVFXDirection.position, ActivePushDirection * 100f);
        
        /*
        bool Manipulating = IsManipulating();
        if (!Manipulating)
            return Vector3.zero;
        */
    }

    public static Vector3 GetAngleHalfway(Vector3 angle1, Vector3 angle2)
    {
        Quaternion q1 = Quaternion.Euler(angle1);
        Quaternion q2 = Quaternion.Euler(angle2);

        // Interpolate between the two quaternions with slerp
        Quaternion halfwayQuat = Quaternion.Slerp(q1, q2, 0.5f);

        // Convert the interpolated quaternion back to Euler angles
        Vector3 halfwayAngle = halfwayQuat.eulerAngles;

        return halfwayAngle;
    }
}
