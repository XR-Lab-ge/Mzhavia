using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class chair : UdonSharpBehaviour
{
    public float maxDistance = 3f;
    private bool active = false;
    private VRCPlayerApi localPlayer;
    private Rigidbody rb;
    private float startY;
    private Quaternion startRotation;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        rb = GetComponent<Rigidbody>();
        startY = transform.position.y;
        startRotation = transform.rotation;
    }

    public override void Interact()
    {
        if (localPlayer == null) return;
        Networking.SetOwner(localPlayer, gameObject);
        active = true;
    }

    void Update()
    {
        float dist = Vector3.Distance(localPlayer.GetPosition(), transform.position);
        if (!active || !Input.GetMouseButton(0) || dist > maxDistance)
        {
            active = false;
            return;
        }

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.W)) moveInput = 1f;
        else if (Input.GetKey(KeyCode.S)) moveInput = -1f;

        if (moveInput != 0)
        {
            Vector3 playerRotation = localPlayer.GetRotation() * Vector3.forward;
            playerRotation.y = 0;
            playerRotation = playerRotation.normalized;

            // Calculate the target position
            Vector3 movement = playerRotation * moveInput * localPlayer.GetWalkSpeed() * Time.deltaTime;
            Vector3 targetPos = transform.position + movement;

            // Lock height
            targetPos.y = startY;

            // Use Rigidbody to move so it stops at walls
            rb.MovePosition(targetPos);
        }

        // Keep upright
        rb.MoveRotation(startRotation);
    }
}