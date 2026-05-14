using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class avatarDoor : UdonSharpBehaviour
{
    [Header("References")]
    public Animator doorAnimator;
    public TicketConductor conductor;
    public string openParam = "Open";

    [Header("Settings")]
    public float closeDelay = 1.5f;

    // hasAccess remains local; only the person clicking needs to pass the check
    public bool hasAccess = false;

    // This is the source of truth for the whole room
    [UdonSynced] private bool _netIsOpen = false;

    private float timer = 0f;
    private bool isCounting = false;

    // --- ACCESS LOGIC (Local Only) ---

    public void TryOpenFromOutside()
    {
        // 1. Check if the cinema is full
        if (conductor != null && conductor.IsCinemaFull())
        {
            Debug.Log("[Door] Cinema Full - Access Blocked.");
            return;
        }

        // 2. Check if this specific player has access
        if (!hasAccess)
        {
            Debug.Log("[Door] No Access - Door remains shut.");
            return;
        }

        // 3. If passed, tell the network to open the door
        SendOpenRequest(true);
    }

    public void TryOpenFromInside()
    {
        // Inside logic: Check if player still has a ticket
        if (conductor != null && conductor.hasTicketLocally)
        {
            Debug.Log("[Door] Locked - You haven't used your ticket yet.");
            return;
        }

        SendOpenRequest(true);
    }

    public void GrantAccess()
    {
        hasAccess = true;
    }

    // --- NETWORKING CORE ---

    private void SendOpenRequest(bool open)
    {
        // Take ownership so we can change the synced variable
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        _netIsOpen = open;

        if (open)
        {
            isCounting = true;
            timer = 0f;
        }
        else
        {
            isCounting = false;
        }

        // Update ourselves immediately
        ApplyVisuals();

        // Update everyone else
        RequestSerialization();
    }

    // This is called automatically on everyone's PC when the owner calls RequestSerialization
    public override void OnDeserialization()
    {
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(openParam, _netIsOpen);
        }
    }

    private void Update()
    {
        // Only the owner processes the countdown timer
        if (isCounting && Networking.IsOwner(gameObject))
        {
            timer += Time.deltaTime;
            if (timer >= closeDelay)
            {
                SendOpenRequest(false);
            }
        }
    }
}