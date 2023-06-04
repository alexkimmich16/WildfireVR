using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
public class XRPlayerMovement : SerializedMonoBehaviour
{
    public static XRPlayerMovement instance;

    [Header("Movement Settings")]
    [SerializeField, FoldoutGroup("Movement Settings")] private float maxVelocity = 5f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float acceleration = 2f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float deceleration = 4f;
    
    [SerializeField, FoldoutGroup("Movement Settings")] private float gravityMultiplier = 1f;
    [SerializeField, FoldoutGroup("Movement Settings")] private float GroundedOffsetCheck = 1f;

    public LayerMask Floors;
    public LayerMask Walls;
    //[SerializeField, FoldoutGroup("Movement Settings"), ReadOnly] private bool UseGravity;

    [Header("Input Settings")]
    [SerializeField, FoldoutGroup("Input Settings")] private XRController controller;
    [ReadOnly, SerializeField, FoldoutGroup("Input Settings")] private Vector2 inputAxis;
    [ReadOnly, SerializeField, FoldoutGroup("Input Settings")] private Vector2 directionAdd;

    private Rigidbody rb;
    private float currentVelocity;
    private float currentAcceleration;
    private float currentDeceleration;
    public bool isGrounded;
    
    private Vector3 groundNormal;

    public XRNode inputSource;

    [ReadOnly, FoldoutGroup("Push")] public float PushAmount;
    [ReadOnly, FoldoutGroup("Push")] public Vector3 PushDirection;
    [FoldoutGroup("Push")] public float SideForceFalloff;
    [FoldoutGroup("Push")] public float UpForceMultiplier;

    [FoldoutGroup("Push")] public List<PushInfo> Pushes = new List<PushInfo>();
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.layer.ToString());
        if(collision.gameObject.layer == 14)
        {
            //Debug.Log("Inside: " + collision.gameObject.layer.ToString());
            Pushes.Clear();
        }
        
    }

    [System.Serializable]
    public struct PushInfo
    {
        public PushInfo(float SideForce, Vector3 Direction)
        {
            this.Direction = Direction;
            this.SideForce = SideForce;
        }
        public Vector3 Direction;
        public float SideForce;
        public Vector3 GetForce() { return new Vector3 (Direction.x * SideForce, 0f, Direction.z * SideForce); }
    }

    public void ManagePushes()
    {
        for (int i = 0; i < Pushes.Count; i++)
        {
            float NewSideForce = Mathf.Clamp(Pushes[i].SideForce - (isGrounded ? Time.deltaTime * SideForceFalloff : 0f), 0f, 100000f);
            Pushes[i] = new PushInfo(NewSideForce, Pushes[i].Direction);
            if (Pushes[i].SideForce <= 0f)
                Pushes.Remove(Pushes[i]);
        }
    }
    private void Update()
    {
        ManagePushes();
    }

    public void Push(float Force, Vector3 Direction)
    {
        rb.AddForce(new Vector3(0f, Direction.y, 0f) * UpForceMultiplier, ForceMode.Impulse);
        if (!isGrounded)
            Pushes.Clear();
        Pushes.Add(new PushInfo(Force, Direction));
    }
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        //SetCollider();
        Vector2 RawInput = GetInput();
        if(RawInput != Vector2.zero)
            inputAxis = RawInput;

        Quaternion headYaw = Quaternion.Euler(0, AIMagicControl.instance.Cam.transform.localEulerAngles.y, 0);
        Vector3 YawAdd = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        directionAdd = new Vector2(YawAdd.x, YawAdd.z);

        Vector3 AdjustedPosition = new Vector3(transform.position.x, transform.position.y + GroundedOffsetCheck, transform.position.z);
        
        // Check if player is grounded
        isGrounded = Physics.Raycast(AdjustedPosition, -transform.up,  out RaycastHit hit, 0.1f, Floors);
        if (isGrounded) groundNormal = hit.normal;

        // Apply gravity
        Vector3 gravity = Physics.gravity * gravityMultiplier;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // Calculate input direction and acceleration
        Vector3 inputDirection = new Vector3(directionAdd.x, 0f, directionAdd.y);
        inputDirection = Quaternion.FromToRotation(Vector3.up, groundNormal) * inputDirection;
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);


        // Apply acceleration and deceleration
        currentAcceleration = GetInput().magnitude > 0.1f ? acceleration : -deceleration;
        currentVelocity = Mathf.Clamp(currentVelocity + currentAcceleration * Time.fixedDeltaTime, 0f, maxVelocity);

        // Apply movement
        Vector3 movement = inputDirection * currentVelocity;
        movement = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement;
        movement.y = rb.velocity.y;

        for (int i = 0; i < Pushes.Count; i++)
        {
            movement += Pushes[i].GetForce();
        }

        rb.velocity = movement;
        return;
        /*
        if (isGrounded)
        {
            // Apply acceleration and deceleration
            currentAcceleration = GetInput().magnitude > 0.1f ? acceleration : -deceleration;
            currentVelocity = Mathf.Clamp(currentVelocity + currentAcceleration * Time.fixedDeltaTime, 0f, maxVelocity);

            // Apply movement
            Vector3 movement = inputDirection * currentVelocity;
            movement = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement;
            movement.y = rb.velocity.y;

            for (int i = 0; i < Pushes.Count; i++)
            {
                movement += Pushes[i].GetForce();
            }

            rb.velocity = movement;
        }
        
        else
        {
            // Apply air movement
            currentVelocity = Mathf.Clamp(currentVelocity + acceleration * Time.fixedDeltaTime, 0f, maxVelocity);
            Vector3 movement = inputDirection * currentVelocity;
            movement = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement;
            for (int i = 0; i < Pushes.Count; i++)
            {
                movement += Pushes[i].GetForce();
            }
            rb.velocity = movement;
            //rb.AddForce(movement, ForceMode.Acceleration);
        }
        */

    }
    public Vector2 GetInput()
    {
        InputDevices.GetDeviceAtXRNode(inputSource).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 RawInput);
        return RawInput;
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
