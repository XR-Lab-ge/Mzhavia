using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class avatarDoor : UdonSharpBehaviour
{
    [Header("References")]
    public Animator doorAnimator;
    public TicketConductor conductor;
    public string openParam = "Open";

    [Header("Settings")]
    public float closeDelay = 1.5f;
    public bool hasAccess = false;

    private bool isOpen = false;
    private float timer = 0f;
    private bool isCounting = false;

    // --- GATEKEEPER LOGIC ---

    public void TryOpenFromOutside()
    {
        // NEW CHECK: Block entry if conductor says we are at 5/5 tickets
        if (conductor != null && conductor.IsCinemaFull())
        {
            Debug.Log("Door: Cinema is FULL. Access blocked.");
            return;
        }

        if (!hasAccess)
        {
            Debug.Log("Door: Access denied.");
            return;
        }

        OpenDoor();
    }

    public void TryOpenFromInside()
    {
        if (conductor != null && conductor.hasTicketLocally)
        {
            Debug.Log("Door: LOCKED! You still have a ticket.");
            return;
        }

        OpenDoor();
    }

    // --- DOOR MECHANICS ---

    public void GrantAccess()
    {
        hasAccess = true;
    }

    private void OpenDoor()
    {
        isOpen = true;
        isCounting = true;
        timer = 0f;
        doorAnimator.SetBool(openParam, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        isCounting = false;
        doorAnimator.SetBool(openParam, false);
    }

    void Update()
    {
        if (isCounting)
        {
            timer += Time.deltaTime;
            if (timer >= closeDelay) CloseDoor();
        }
    }
}