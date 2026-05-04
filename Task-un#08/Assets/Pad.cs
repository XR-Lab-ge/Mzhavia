using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float upwardForce = 10f;
    public float forwardForce = 8f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // reset velocity for consistent boost
                rb.velocity = Vector3.zero;

                // direction based on JumpPad orientation
                Vector3 launchDirection = transform.forward * forwardForce + Vector3.up * upwardForce;

                rb.AddForce(launchDirection, ForceMode.Impulse);
            }
        }
    }
}