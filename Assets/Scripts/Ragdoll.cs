using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public bool RagdollState;
    private bool Last;
    public float Upforce;
    public float SideForce;
    public Rigidbody[] Rigidbodies;
    public PlayerControl SubscribeDisolve;
    private bool Started = false;
    public void EnableRagdoll()
    {
        foreach (Rigidbody rb in Rigidbodies)
        {
            rb.AddForce(Vector3.up * Upforce, ForceMode.Impulse);
            rb.AddForce(Vector3.left * SideForce, ForceMode.Impulse);
            rb.isKinematic = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (Rigidbody rb in Rigidbodies)
        {
            
            rb.isKinematic = true;
        }
    }
    void Start()
    {
        Last = RagdollState;

        Rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
        TryInitialize();
    }
     
    public void LookForSubscribe()
    {
        try
        {
            Transform Myobject = GameObject.Find("My Player").transform;
            if (Myobject != null)
            {
                SubscribeDisolve = Myobject.GetComponent<PlayerControl>();
            }
        }
        catch
        {

        }
    }
    // Update is called once per frame
    void Update()
    {
        if(RagdollState == true && Last == false)
        {
            EnableRagdoll();
        }
        else if (RagdollState == false && Last == true)
        {
            DisableRagdoll();
        }

        if (Started == false)
        {
            LookForSubscribe();
            TryInitialize();
        }
    }
    public void TryInitialize()
    {
        if (SubscribeDisolve != null)
        {
            Started = true;
            SubscribeDisolve.disolveEvent += EnableRagdoll;
        }
    }


}
