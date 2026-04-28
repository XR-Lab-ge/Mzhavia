using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class door : UdonSharpBehaviour
{
    public Animator animator;

    private bool isOpen = false;
    private bool busy = false;

    public override void Interact()
    {
        if (busy) return;

        busy = true;

        isOpen = !isOpen;

        animator.ResetTrigger("Open");
        animator.ResetTrigger("Close");

        if (isOpen)
        {
            animator.SetTrigger("Open");
        }
        else
        {
            animator.SetTrigger("Close");
        }

        SendCustomEventDelayedSeconds(nameof(ResetBusy), 0.2f);
    }

    public void ResetBusy()
    {
        busy = false;
    }
}