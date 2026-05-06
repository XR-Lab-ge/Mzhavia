using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class chair : UdonSharpBehaviour
{
    public VRC.SDK3.Components.VRCStation station;

    // We create a custom public method for the UI
    public void _SitInChair()
    {
        Debug.Log("UI Button Clicked");

        if (station == null)
        {
            Debug.Log("Station is NULL");
            return;
        }

        VRCPlayerApi player = Networking.LocalPlayer;

        if (player == null) return;

        // Forces the player into the station
        station.UseStation(player);
    }

    // Optional: Keep this if you want BOTH the button and the physical chair to work
    public override void Interact()
    {
        _SitInChair();
    }
}