using UnityEngine;

public class RigidbodyThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float groundCheckRadius = 0.25f;

    [Header("Ground")]
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0, z).normalized;

        // Ground check 
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask
        );

        // Jump
        
        
       if (Input.GetButtonDown("Jump"))
       {
          rb.velocity = new Vector3(rb.velocity.x, 7f, rb.velocity.z);
       }
        
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        Vector3 moveDir = GetCameraRelative(moveInput);

        Vector3 velocity = rb.velocity;
        Vector3 targetVelocity = moveDir * moveSpeed;

        rb.velocity = new Vector3(targetVelocity.x, velocity.y, targetVelocity.z);
    }

    void Rotate()
    {
        Vector3 moveDir = GetCameraRelative(moveInput);

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void Jump()
    {
        // reset Y for jump
        Vector3 vel = rb.velocity;
        vel.y = 0f;
        rb.velocity = vel;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    Vector3 GetCameraRelative(Vector3 input)
    {
        if (Camera.main == null) return input;

        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * input.z + right * input.x;
    }
}