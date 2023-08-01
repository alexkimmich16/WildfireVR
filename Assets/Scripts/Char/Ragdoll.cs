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
    }
    [Button]
    public void DoAnim()
    {
        StartCoroutine(AnimSequence());
        
    }

    public IEnumerator AnimSequence()
    {
        IK.enabled = false;
        punchAnim.Play("RightHook", 0);
        Debug.Log("issue1: " + IK.enabled);
        //wait until finished
        yield return new WaitWhile(() => !punchAnim.IsInTransition(0));
        Debug.Log("issue2" + IK.enabled);
        IK.enabled = true;
        Debug.Log("issue3" + IK.enabled);
    }
}
