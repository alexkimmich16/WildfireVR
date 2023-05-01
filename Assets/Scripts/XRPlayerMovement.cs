using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxVelocity = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 4f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Input Settings")]
    [SerializeField] private XRController controller;
    [SerializeField] private Vector2 inputAxis;
    [SerializeField] private Vector2 directionAdd;

    private Rigidbody rb;
    private float currentVelocity;
    private float currentAcceleration;
    private float currentDeceleration;
    private bool isGrounded;
    private Vector3 groundNormal;

    public XRNode inputSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        Quaternion headYaw = Quaternion.Euler(0, AIMagicControl.instance.Cam.transform.localEulerAngles.y, 0);
        Vector3 YawAdd = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        directionAdd = new Vector2(YawAdd.x, YawAdd.z);
        // Check if player is grounded
        isGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.1f);
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

        // Jump
        if (isGrounded && controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
        {
            rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
        }
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
