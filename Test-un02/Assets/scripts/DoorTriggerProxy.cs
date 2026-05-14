using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class DoorTriggerProxy : UdonSharpBehaviour
{
    public avatarDoor doorScript;
    public bool isInsideTrigger;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal || doorScript == null) return;

        if (isInsideTrigger)
        {
            Debug.Log("<color=blue>[Proxy]</color> Inside Trigger hit. Telling door to check ticket.");
            doorScript.TryOpenFromInside();
        }
        else
        {
            Debug.Log("<color=green>[Proxy]</color> Outside Trigger hit. Telling door to check access.");
            doorScript.TryOpenFromOutside();
        }
    }
}