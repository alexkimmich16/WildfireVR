using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;

    public void Map(Vector3 Pos, Vector3 Rot)
    {
        rigTarget.position = vrTarget.TransformPoint(Pos);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(Rot);
    }
}
[System.Serializable]
public class VRType
{
    public string type;
    public Vector3 ControllerPosOffset;
    public Vector3 ControllerRotOffset;
}

public class VRAnimator : MonoBehaviour
{
    public float TurnSmooth;
    public VRMap head;
    public VRMap left;
    public VRMap right;

    public Transform headConstraint;
    public Vector3 headBodyOffset;

    public int VRType;
    public List<VRType> VRTypes;

    private Animator anim;
    [Range(0f, 1f)]
    public float WalkingThreshold;

    [Range(0f, 1f)]
    public float Current;

    public Vector3 HeadPosOffset;
    public Vector3 HeadRotOffset;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized, Time.deltaTime * TurnSmooth);

        VRType T = VRTypes[VRType];
        head.Map(HeadPosOffset, HeadRotOffset);
        left.Map(new Vector3(-T.ControllerPosOffset.x, T.ControllerPosOffset.y, T.ControllerPosOffset.z), T.ControllerRotOffset);
        right.Map(new Vector3(T.ControllerPosOffset.x, T.ControllerPosOffset.y, T.ControllerPosOffset.z), T.ControllerRotOffset);
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
