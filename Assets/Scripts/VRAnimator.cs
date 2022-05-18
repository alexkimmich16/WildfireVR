using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRAnimator : MonoBehaviour
{
    public float TurnSmooth;
    public VRMap head;
    public VRMap left;
    public VRMap right;

    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public Vector3 CameraOffset;

    public Animator anim;
    [Range(0f, 1f)]
    public float WalkingThreshold;

    [Range(0f, 1f)]
    public float Current;
    void Update()
    {
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized, Time.deltaTime * TurnSmooth);

        head.Map();
        left.Map();
        right.Map();
        Current = Vector2.SqrMagnitude(MovementProvider.instance.inputAxis);
        //Debug.Log(MoveSpeed);
        if (anim == null)
            return;
        if (Current < WalkingThreshold)
            anim.SetBool("Walking", false);
        else
            anim.SetBool("Walking", true);

    }
}
