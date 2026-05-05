using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class chair : UdonSharpBehaviour
{
    public VRC.SDK3.Components.VRCStation station;

    public override void Interact()
    {
        Debug.Log("Interact fired");

        if (station == null)
        {
            Debug.Log("Station is NULL");
            return;
        }

        var player = Networking.LocalPlayer;

        if (player == null)
        {
            Debug.Log("No local player");
            return;
        }

        Debug.Log("Forcing station use");
        station.UseStation(player);
    }
}