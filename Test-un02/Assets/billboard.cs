using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class billboard : UdonSharpBehaviour
{
    void LateUpdate()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            // Makes the object look at the player's head
            transform.LookAt(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position);

            // UI usually faces backwards after a LookAt, so flip it 180 degrees
            transform.Rotate(0, 180, 0);
        }
    }
}