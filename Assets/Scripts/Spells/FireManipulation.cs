using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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


    public float Additional;

    List<Vector3> BothHandMotions;
    public int LastFrames;


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
        //Vector3 RightVelocity = PastFrameRecorder.instance.GetControllerInfo(Side.right).HandPos - PastFrameRecorder.instance.PastFrame(Side.right).HandPos;
        //Vector3 LeftVelocity = PastFrameRecorder.instance.GetControllerInfo(Side.left).HandPos - PastFrameRecorder.instance.PastFrame(Side.left).HandPos;
        //Vector3 ForwardHands = GetAngleHalfway(RightVelocity, LeftVelocity);


        //float HeadSetForward = AIMagicControl.instance.Cam.rotation.y;
       // Vector2 HeadRot = ToVector(HeadSetForward * Additional * 90);


        

        

        //Vector3 FinalOutput = new Vector3(ForwardHands.x + HeadRot.x, ForwardHands.y, ForwardHands.z + HeadRot.y);

        //Debug.Log("Hand: " + ForwardHands.y + "  head: " + HeadSetForward);

        //Distance / (frame2.SpawnTime - frame1.SpawnTime)
        //InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 RightVelocity);
        //InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 LeftVelocity);

        //InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 RightPos);
        //InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 LeftPos);

        return Vector3.down;
        //if()
    }
    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180f / Mathf.PI; }
    public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }
    // Update is called once per frame
    void Update()
    {
        if (!Athena.PastFrameRecorder.IsReady || !AIMagicControl.instance.AllActive())
            return;
        BothHandMotions.Add((AIMagicControl.instance.Hands[0].position + AIMagicControl.instance.Hands[1].position) / 2f);
        if (BothHandMotions.Count < 2)
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
        ActivePushDirection = (BothHandMotions[^1] - BothHandMotions[0]).normalized;

        if (BothHandMotions.Count > LastFrames)
        {
            BothHandMotions.RemoveAt(0);
        }
        AmbientVFXDirection.forward = ActivePushDirection;
        //AmbientVFXDirection.forward = (PastFrameRecorder.instance.GetControllerInfo(Side.right).HandPos - PastFrameRecorder.instance.PastFrame(Side.right).HandPos).normalized;
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
