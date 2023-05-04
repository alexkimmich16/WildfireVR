using UnityEngine;
using Sirenix.OdinInspector;
public class Ragdoll : SerializedMonoBehaviour
{
    public float Upforce;
    public float SideForce;
    public Rigidbody[] Rigidbodies;
    public Collider[] Colliders;

    public RootMotion.FinalIK.VRIK IK;

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
