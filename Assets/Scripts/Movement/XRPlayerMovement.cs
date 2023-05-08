using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Sirenix.OdinInspector;
public class XRPlayerMovement : SerializedMonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, FoldoutGroup("Movement Settings")] private float maxVelocity = 5f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float acceleration = 2f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float deceleration = 4f;
    
    [SerializeField, FoldoutGroup("Movement Settings")] private float gravityMultiplier = 1f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float GroundedOffsetCheck = 1f;
    //[SerializeField, FoldoutGroup("Movement Settings"), ReadOnly] private bool UseGravity;

    [Header("Input Settings")]
    [SerializeField, FoldoutGroup("Input Settings")] private XRController controller;
    [ReadOnly, SerializeField, FoldoutGroup("Input Settings")] private Vector2 inputAxis;
    [ReadOnly, SerializeField, FoldoutGroup("Input Settings")] private Vector2 directionAdd;

    private Rigidbody rb;
    private float currentVelocity;
    private float currentAcceleration;
    private float currentDeceleration;
    private bool isGrounded;
    
    private Vector3 groundNormal;

    public XRNode inputSource;

    [ReadOnly, FoldoutGroup("Push")] public float PushAmount;
    [ReadOnly, FoldoutGroup("Push")] public Vector3 PushDirection;

    //public void SetCollider() { getx}
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        //SetCollider();

        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        Quaternion headYaw = Quaternion.Euler(0, AIMagicControl.instance.Cam.transform.localEulerAngles.y, 0);
        Vector3 YawAdd = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        directionAdd = new Vector2(YawAdd.x, YawAdd.z);

        Vector3 AdjustedPosition = new Vector3(transform.position.x, transform.position.y + GroundedOffsetCheck, transform.position.z);
        
        // Check if player is grounded
        isGrounded = Physics.Raycast(AdjustedPosition, -transform.up, out RaycastHit hit, 0.1f);
        if (isGrounded)
        {
            groundNormal = hit.normal;
        }

        // Apply gravity
        Vector3 gravity = Physics.gravity * gravityMultiplier;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // Calculate input direction and acceleration
        Vector3 inputDirection = new Vector3(directionAdd.x, 0f, directionAdd.y);
        inputDirection = Quaternion.FromToRotation(Vector3.up, groundNormal) * inputDirection;
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);
        Vector3 accelerationDirection = inputDirection.normalized;

        if (isGrounded)
        {
            // Apply acceleration and deceleration
            currentAcceleration = inputDirection.magnitude > 0.1f ? acceleration : deceleration;
            currentVelocity = Mathf.Clamp(currentVelocity + currentAcceleration * Time.fixedDeltaTime, 0f, maxVelocity);

            // Apply movement
            Vector3 movement = inputDirection * currentVelocity;
            movement = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
        else
        {
            // Apply air movement
            currentVelocity = Mathf.Clamp(currentVelocity + acceleration * Time.fixedDeltaTime, 0f, maxVelocity);
            Vector3 movement = inputDirection * currentVelocity;
            movement = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement;
            rb.AddForce(movement, ForceMode.Acceleration);
        }
        /*
        // Jump
        if (isGrounded && controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
        {
            rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
        }
        */
    }

    public void SetInputAxis(Vector2 axis)
    {
        if (Mathf.Sign(axis.x) == Mathf.Sign(inputAxis.x))
        {
            // Same direction, accelerate
            inputAxis = axis;
        }
        else
        {
            // Opposite
            inputAxis = axis;
            currentVelocity = 0f;
            currentAcceleration = acceleration;
        }
        // Opposite direction, stop and change direction
        
    }
    
}
//[SerializeField] private float jumpForce = 5f;