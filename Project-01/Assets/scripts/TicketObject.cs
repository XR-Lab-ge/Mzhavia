using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

public class TicketObject : UdonSharpBehaviour
{
    public TicketConductor conductor;
    public TextMeshPro textElement;

    [HideInInspector] public int myIndex;
    [UdonSynced] private bool isTicketVisible = false;

    // Call this to spawn the ticket correctly
    public void SetupAndShow(int number)
    {
        // 1. Take Networking Ownership immediately
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

        // 2. Set the Visuals
        if (textElement != null) textElement.text = number.ToString();
        isTicketVisible = true;
        gameObject.SetActive(true);

        // 3. Sync to others
        RequestSerialization();

        // 4. Position Calibration (Spawns 0.4 meters in front of your eyes)
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp != null)
        {
            Vector3 headPos = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Quaternion headRot = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            this.transform.position = headPos + (headRot * Vector3.forward * 0.4f);
            this.transform.rotation = headRot;
        }
    }

    public override void OnPickupUseDown()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        Vector3 eyePos = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 eyeDir = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;

        RaycastHit hit;
        // Only do something if we hit our specific chair
        if (Physics.Raycast(eyePos, eyeDir, out hit, 2.5f))
        {
            NumberedSeat seat = (NumberedSeat)hit.collider.gameObject.GetComponent(typeof(UdonBehaviour));
            if (seat != null && seat.seatNumber == (myIndex + 1))
            {
                seat.station.UseStation(lp);
                // Ticket stays in hand! No disappearance here.
                return;
            }
        }

        // If we hit the wrong chair or nothing, we do NOTHING. Ticket stays.
    }

    public override void OnDeserialization()
    {
        gameObject.SetActive(isTicketVisible);
    }
}