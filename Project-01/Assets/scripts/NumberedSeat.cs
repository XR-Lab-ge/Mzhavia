using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NumberedSeat : UdonSharpBehaviour
{
    [Header("Ticket Info")]
    public int seatNumber;
    public TicketConductor conductor;

    [Header("The Chair")]
    public VRCStation station; // You will drag the Chair here

    // This runs the moment your laser clicks the button's collider
    public override void Interact()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp == null || conductor == null || station == null) return;

        // Check the ticket
        if (conductor.IsThisPlayersTicket(seatNumber))
        {
            // Pull the player into the seat!
            station.UseStation(lp);
        }
        else
        {
            Debug.Log("Access Denied for Seat #" + seatNumber);
        }
    }
}