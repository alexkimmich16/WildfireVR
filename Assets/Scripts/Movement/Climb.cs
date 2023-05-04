using UnityEngine;

public class Climb : MonoBehaviour
{
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
}
