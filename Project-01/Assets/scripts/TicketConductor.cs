using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TicketConductor : UdonSharpBehaviour
{
    [Header("References")]
    public avatarDoor doorScript;
    public TicketObject[] ticketPool;

    [Header("Network Sync")]
    [UdonSynced] private int takenMask = 0;

    public bool hasTicketLocally = false;
    private int localPlayerAssignedNumber = -1;

    public override void Interact()
    {
        // 1. If player ALREADY has a ticket, pressing E returns it
        if (hasTicketLocally)
        {
            Debug.Log("Conductor: Returning ticket #" + localPlayerAssignedNumber);

            int indexToReturn = localPlayerAssignedNumber - 1;
            ReturnTicket(indexToReturn);

            if (ticketPool[indexToReturn] != null)
            {
                ticketPool[indexToReturn].gameObject.SetActive(false);
            }
            return;
        }

        // 2. Access Check
        if (doorScript != null && !doorScript.hasAccess)
        {
            Debug.Log("Conductor: Access denied. Use the pedestal first.");
            return;
        }

        // 3. Normal logic to ISSUE a ticket
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
        }
    }

    public void ReturnTicket(int index)
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

        takenMask &= ~(1 << index);
        RequestSerialization();

        hasTicketLocally = false;
        localPlayerAssignedNumber = -1;

        Debug.Log("Conductor: Ticket #" + (index + 1) + " is now available again.");
    }

    public bool IsThisPlayersTicket(int checkNumber)
    {
        return localPlayerAssignedNumber == checkNumber;
    }

    // NEW FUNCTION: Checks if all 5 bits are set (11111 in binary = 31)
    public bool IsCinemaFull()
    {
        return takenMask >= 31;
    }
}