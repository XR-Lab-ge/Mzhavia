using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float acceleration = 50f;
    public float deceleration = 10f;
    public float rotationSpeed = 15f;

    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float groundCheckRadius = 0.3f;
    public float fallMultiplier = 2.5f;

    [Header("Physics/Ground")]
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;
    private bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Setup Rigidbody for smooth movement
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. Get Input (Normalized prevents fast diagonal movement)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0, z).normalized;

        // 2. Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // 3. Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
        ApplyJump();
        ApplyBetterFall();
    }

    void ApplyMovement()
    {
        // Get camera-relative direction
        Vector3 moveDir = GetCameraRelative(moveInput);

        // Target velocity on the XZ plane
        Vector3 targetVelocity = moveDir * moveSpeed;

        // Calculate velocity difference
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Vector3 velocityChange = (targetVelocity - horizontalVelocity);

        // Clamp the force to avoid infinite acceleration
        velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -acceleration, acceleration);
        velocityChange.y = 0;

        // Apply force
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Artificial friction to stop sliding when no keys are pressed
        if (moveInput.magnitude < 0.1f && isGrounded)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), deceleration * Time.fixedDeltaTime);
        }
    }

    void ApplyRotation()
    {
        Vector3 moveDir = GetCameraRelative(moveInput);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void ApplyJump()
    {
        if (jumpRequested)
        {
            // Reset Y velocity for consistent jump height
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    void ApplyBetterFall()
    {
        // Makes falling faster than rising (standard platformer feel)
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    Vector3 GetCameraRelative(Vector3 input)
    {
        if (Camera.main == null) return input;

        Transform cam = Camera.main.transform;

        // Project vectors onto flat plane to fix the "backward/downward" camera bug
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        return (forward * input.z) + (right * input.x);
    }

    // Visual helper in the Editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }
}