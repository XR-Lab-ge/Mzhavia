using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class door : UdonSharpBehaviour
{
    [Header("Animator with open/close animation")]
    public Animator doorAnimator;

    [Header("Animation parameter name")]
    public string openParam = "Open";

    [Header("Delay before closing")]
    public float closeDelay = 1f;

    // SYNCING: This ensures every player knows if the door is open.
    [UdonSynced, FieldChangeCallback(nameof(isOpen))]
    private bool _isOpen = false;

    public bool isOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            if (doorAnimator != null)
            {
                doorAnimator.SetBool(openParam, _isOpen);
            }
        }
    }

    private float timer = 0f;
    private int playersInTrigger = 0; // Track count to support multiple people

    void Update()
    {
        // Only the owner of the door handles the closing timer logic
        if (!Networking.IsOwner(gameObject)) return;

        if (isOpen && playersInTrigger <= 0)
        {
            timer += Time.deltaTime;
            if (timer >= closeDelay)
            {
                isOpen = false;
                RequestSerialization();
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // We count EVERY player that enters, but only the owner changes the variable
        playersInTrigger++;

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        isOpen = true;
        timer = 0f;
        RequestSerialization();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        playersInTrigger--;
        if (playersInTrigger < 0) playersInTrigger = 0;

        if (playersInTrigger <= 0)
        {
            timer = 0f; // Reset timer to start the countdown in Update()
        }
    }
}