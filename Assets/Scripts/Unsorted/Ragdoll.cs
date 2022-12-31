using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public float Upforce;
    public float SideForce;
    public Rigidbody[] Rigidbodies;
    public Collider[] Colliders;

    public RootMotion.FinalIK.VRIK IK;
    public void EnableRagdoll()
    {
        //seperate from playerw
        //DISABLE IK

        IK.enabled = false;
        foreach (Rigidbody rb in Rigidbodies)
        {
            rb.AddForce(Vector3.up * Upforce, ForceMode.Impulse);
            rb.AddForce(Vector3.left * SideForce, ForceMode.Impulse);
            rb.isKinematic = false;
        }
        foreach (Collider Col in Colliders)
        {
            Col.enabled = true;
        }
    }

    public void DisableRagdoll()
    {
        IK.enabled = true;
        foreach (Rigidbody rb in Rigidbodies)
        {
            rb.isKinematic = true;
        }
        foreach (Collider Col in Colliders)
        {
            Col.enabled = false;
        }
    }
    void Start()
    {
        DisableRagdoll();
    }
}
