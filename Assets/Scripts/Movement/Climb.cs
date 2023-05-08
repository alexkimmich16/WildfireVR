using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Climb : MonoBehaviour
{
    [Header("Steps")]
    public float maxStepHeight = 0.4f;              //The maximum a player can set upwards in units when they hit a wall that's potentially a step
    public float stepSearchOvershoot = 0.01f;       //How much to overshoot into the direction a potential step in units when testing. High values prevent player from walking up small steps but may cause problems.

    private List<ContactPoint> allCPs = new List<ContactPoint>();
    private Vector3 lastVelocity;

    void FixedUpdate()
    {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;

        //Filter through the ContactPoints to see if we're grounded and to see if we can step up
        ContactPoint groundCP = default(ContactPoint);
        bool grounded = FindGround(out groundCP, allCPs);

        Vector3 stepUpOffset = default(Vector3);
        bool stepUp = false;
        if (grounded)
            stepUp = FindStep(out stepUpOffset, allCPs, groundCP, velocity);

        //Steps
        if (stepUp)
        {
            this.GetComponent<Rigidbody>().position += stepUpOffset;
            this.GetComponent<Rigidbody>().velocity = lastVelocity;
        }

        allCPs.Clear();
        lastVelocity = velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    void OnCollisionStay(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    /// Finds the MOST grounded (flattest y component) ContactPoint
    /// \param allCPs List to search
    /// \param groundCP The contact point with the ground
    /// \return If grounded
    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCPs)
    {
        groundCP = default(ContactPoint);
        bool found = false;
        foreach (ContactPoint cp in allCPs)
        {
            //Pointing with some up direction
            if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
            {
                groundCP = cp;
                found = true;
            }
        }

        return found;
    }

    /// Find the first step up point if we hit a step
    /// \param allCPs List to search
    /// \param stepUpOffset A Vector3 of the offset of the player to step up the step
    /// \return If we found a step
    bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCPs, ContactPoint groundCP, Vector3 currVelocity)
    {
        stepUpOffset = default(Vector3);

        //No chance to step if the player is not moving
        Vector2 velocityXZ = new Vector2(currVelocity.x, currVelocity.z);
        if (velocityXZ.sqrMagnitude < 0.0001f)
            return false;

        foreach (ContactPoint cp in allCPs)
        {
            bool test = ResolveStepUp(out stepUpOffset, cp, groundCP);
            if (test)
                return test;
        }
        return false;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
    /// \param stepTestCP ContactPoint to check.
    /// \param groundCP ContactPoint on the ground.
    /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
    /// \return If the passed ContactPoint was a step
    bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Collider stepCol = stepTestCP.otherCollider;

        //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
        if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
        {
            return false;
        }

        //( 2 ) Make sure the contact point is low enough to be a step
        if (!(stepTestCP.point.y - groundCP.point.y < maxStepHeight))
        {
            return false;
        }

        //( 3 ) Check to see if there's actually a place to step in front of us
        //Fires one Raycast
        RaycastHit hitInfo;
        float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if (!(stepCol.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)))
        {
            return false;
        }

        //We have enough info to calculate the points
        Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y + 0.0001f, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);

        //We passed all the checks! Calculate and return the point!
        stepUpOffset = stepUpPointOffset;
        return true;
    }

    /*
    Rigidbody rigidBody;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        
    }

    private void FixedUpdate()
    {
        stepClimb();
    }

    void stepClimb()
    {
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.localPosition.x, stepHeight, stepRayUpper.transform.localPosition.z);

        //stepRayUpper.transform.forward = new Vector3(0, AIMagicControl.instance.Cam.rotation.eulerAngles.y, 0);
        //stepRayLower.transform.forward = new Vector3(0, AIMagicControl.instance.Cam.rotation.eulerAngles.y, 0);
        //stepRayUpper.transform.forward = new vector3(AIMagicControl.instance.Cam.transform.rotation.eulerAngles.);  

        Debug.DrawLine(stepRayLower.transform.position, (AIMagicControl.instance.Cam.TransformDirection(Vector3.forward) * 0.1f) + stepRayLower.transform.position);
        Debug.DrawLine(stepRayUpper.transform.position, (AIMagicControl.instance.Cam.TransformDirection(Vector3.forward) * 0.1f) + stepRayLower.transform.position);

        Debug.DrawLine(stepRayLower.transform.position, (AIMagicControl.instance.Cam.TransformDirection(1.5f, 0, 1) * 0.1f) + stepRayLower.transform.position);
        Debug.DrawLine(stepRayUpper.transform.position, (AIMagicControl.instance.Cam.TransformDirection(1.5f, 0, 1) * 0.1f) + stepRayLower.transform.position);

        Debug.DrawLine(stepRayLower.transform.position, (AIMagicControl.instance.Cam.TransformDirection(-1.5f, 0, 1) * 0.1f) + stepRayLower.transform.position);
        Debug.DrawLine(stepRayUpper.transform.position, (AIMagicControl.instance.Cam.TransformDirection(-1.5f, 0, 1) * 0.1f) + stepRayLower.transform.position);

        //no lower
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, AIMagicControl.instance.Cam.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, AIMagicControl.instance.Cam.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                Debug.Log("MovePOS");
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, AIMagicControl.instance.Cam.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
        {

            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, AIMagicControl.instance.Cam.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
            {
                Debug.Log("MovePOS");
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, AIMagicControl.instance.Cam.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
        {

            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, AIMagicControl.instance.Cam.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
            {
                Debug.Log("MovePOS");
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }
    }
    */
}
