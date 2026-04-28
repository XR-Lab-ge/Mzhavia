using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class chair : UdonSharpBehaviour
{
    public float pushDistance = 0.2f;
    public float smooth = 6f;

    private Vector3 targetPos;
    private bool moving;
    private bool cooldown;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * smooth
            );

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                moving = false;
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (cooldown) return;

        cooldown = true;

        Vector3 dir = (transform.position - player.GetPosition()).normalized;
        dir.y = 0;

        targetPos = transform.position + dir * pushDistance;
        moving = true;

        SendCustomEventDelayedSeconds(nameof(Reset), 0.3f);
    }

    public void Reset()
    {
        cooldown = false;
    }
}