using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class door: UdonSharpBehaviour
{
    [Header("Animator with open/close animation")]
    public Animator doorAnimator;

    [Header("Animation parameter name")]
    public string openParam = "Open";

    [Header("Delay before closing")]
    public float closeDelay = 1f;

    private bool isOpen = false;
    private float timer = 0f;

    private bool playerInside = false;

    void Update()
    {
        // If door is open and player is not inside, start timer
        if (isOpen && !playerInside)
        {
            timer += Time.deltaTime;

            if (timer >= closeDelay)
            {
                CloseDoor();
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        playerInside = true;

        OpenDoor();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        playerInside = false;
        timer = 0f; // reset close timer when leaving trigger
    }

    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        timer = 0f;

        doorAnimator.SetBool(openParam, true);
    }

    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;

        doorAnimator.SetBool(openParam, false);
    }
}