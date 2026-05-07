using UdonSharp;

using UnityEngine;

using VRC.SDKBase;

using VRC.Udon;



public class pedestalTrigger : UdonSharpBehaviour

{

    public avatarDoor door;



    public override void Interact()

    {

        door.GrantAccess();

        Debug.Log("Pedestal used → access sent to door");

    }

}