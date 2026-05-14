using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NumberedSeat : UdonSharpBehaviour
{
    public int seatNumber;
    public VRCStation station;
    // No Interact() logic needed! The Ticket handles the click.
}