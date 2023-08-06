using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
public class Ragdoll : SerializedMonoBehaviour
{
    public float Upforce;
    public float SideForce;
    public Rigidbody[] Rigidbodies;
    public Collider[] Colliders;

    public RootMotion.FinalIK.VRIK IK;

    public Animator punchAnim;

    [Button(ButtonSizes.Small)]

    public void EnableRagdoll()
    {
        DoRagdoll();
        DoRagdoll();
    }
    public void DoRagdoll()
    {
        IK.enabled = false;
        SetAnim(false);
        foreach (Rigidbody rb in Rigidbodies)
        {
            rb.AddForce(Vector3.up * Upforce, ForceMode.VelocityChange);

            Vector3 Backwards = -transform.forward;
            rb.AddForce(Backwards * SideForce, ForceMode.VelocityChange);
            rb.isKinematic = false;
        }
        foreach (Collider Col in Colliders)
        {
            Col.enabled = true;
        }
    }
    [Button(ButtonSizes.Small)]
    public void DisableRagdoll()
    {
        if (IK == null)
            return;
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
        SetAnim(false);
    }

    [Button]
    public void DoAnim()
    {
        StartCoroutine(AnimSequence());
    }

    public void SetAnim(bool state)
    {
        //Debug.Log("state: " + state);
        if(punchAnim != null)
            punchAnim.enabled = state;
    }

    public IEnumerator AnimSequence()
    {
        SetAnim(true);
        IK.enabled = false;
        punchAnim.Rebind();
        //wait until finished

        yield return new WaitWhile(() => !punchAnim.GetCurrentAnimatorStateInfo(0).IsName("AfterHook"));
        IK.enabled = true;
        SetAnim(false);
    }
}
