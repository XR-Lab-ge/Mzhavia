using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TicketConductor : UdonSharpBehaviour
{
    [Header("References")]
    public avatarDoor doorScript;
    public TicketObject[] ticketPool;

    [Header("SFX (Optional)")]
    public AudioSource soundSource;
    public AudioClip getTicketSound;
    public AudioClip denySound;
    public AudioClip returnSound;

    [Header("Network Sync")]
    [UdonSynced] private int takenMask = 0;

    public bool hasTicketLocally = false;
    private int localPlayerAssignedNumber = -1;

    void Start()
    {
        // Initial text setup
        UpdateInteractionText();
    }

    // This handles the text refresh for both PC and Android
    public void UpdateInteractionText()
    {
        string newText = "Take Cinema Ticket";

        if (hasTicketLocally)
        {
            newText = "Return Ticket #" + localPlayerAssignedNumber;
        }
        else if (doorScript != null && !doorScript.hasAccess)
        {
            newText = "Access Denied: Use Pedestal";
        }
        else if (IsCinemaFull())
        {
            newText = "Cinema Full (5/5)";
        }

        this.InteractionText = newText;
    }

    public override void Interact()
    {
        // 1. RETURN LOGIC
        if (hasTicketLocally)
        {
            int indexToReturn = localPlayerAssignedNumber - 1;
            ReturnTicket(indexToReturn);

            if (ticketPool[indexToReturn] != null)
            {
                ticketPool[indexToReturn].gameObject.SetActive(false);
            }

            PlaySound(returnSound);
            // Refresh text immediately for the local player
            UpdateInteractionText();
            return;
        }

        // 2. ACCESS CHECK
        if (doorScript != null && !doorScript.hasAccess)
        {
            PlaySound(denySound);
            UpdateInteractionText();
            return;
        }

        // 3. FULL CHECK
        if (IsCinemaFull())
        {
            PlaySound(denySound);
            UpdateInteractionText();
            return;
        }

        // 4. ISSUE TICKET
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

        int assignedIndex = -1;
        for (int i = 0; i < 5; i++)
        {
            if (((takenMask >> i) & 1) == 0)
            {
                assignedIndex = i;
                break;
            }
        }

        if (assignedIndex != -1)
        {
            takenMask |= (1 << assignedIndex);
            RequestSerialization();

            hasTicketLocally = true;
            localPlayerAssignedNumber = assignedIndex + 1;

            TicketObject card = ticketPool[assignedIndex];
            Networking.SetOwner(Networking.LocalPlayer, card.gameObject);
            card.myIndex = assignedIndex;
            card.SetupAndShow(assignedIndex + 1);

            PlaySound(getTicketSound);
            UpdateInteractionText();
        }
    }

    public void ReturnTicket(int index)
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

        takenMask &= ~(1 << index);
        RequestSerialization();

        hasTicketLocally = false;
        localPlayerAssignedNumber = -1;
        UpdateInteractionText();
    }

    private void PlaySound(AudioClip clip)
    {
        if (soundSource != null && clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }

    public bool IsThisPlayersTicket(int checkNumber)
    {
        return localPlayerAssignedNumber == checkNumber;
    }

    public bool IsCinemaFull()
    {
        return takenMask >= 31;
    }

    public override void OnDeserialization()
    {
        // On Android, tooltips can fail if updated at the exact same millisecond
        // as a network sync. We add a tiny delay to ensure the box is visible.
        SendCustomEventDelayedFrames(nameof(_DelayedTextUpdate), 1);
    }

    public void _DelayedTextUpdate()
    {
        UpdateInteractionText();
    }
}