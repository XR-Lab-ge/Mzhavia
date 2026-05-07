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

    void Start()
    {
        gameObject.SetActive(false);
    }

    // This runs on the client that gets the ticket
    public void SetupAndShow(int number)
    {
        if (textElement != null) textElement.text = number.ToString();

        // Show the object
        gameObject.SetActive(true);

        // Teleport to face so you can pick it up
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp != null)
        {
            Vector3 headPos = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Quaternion headRot = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            // Spawn 0.4 meters in front of face
            this.transform.position = headPos + (headRot * Vector3.forward * 0.4f);
            this.transform.rotation = headRot;
        }
    }

    // Normally we'd return on Drop, but let's make it easy.
    // If you use the card (Trigger/Use button), it returns.
    public override void OnPickupUseDown()
    {
        conductor.ReturnTicket(myIndex);
        gameObject.SetActive(false);
    }
}