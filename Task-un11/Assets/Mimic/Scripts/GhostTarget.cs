using UnityEngine;

public class GhostTarget : MonoBehaviour
{
    [HideInInspector]
    public Transform mimicTransform;
    private CapsuleCollider ghostCollider;

    void Start()
    {
        // 1. Automatically configure our click-detection collider
        ghostCollider = gameObject.AddComponent<CapsuleCollider>();
        ghostCollider.isTrigger = true;
        ghostCollider.radius = 0.6f;
        ghostCollider.height = 2.0f;

        // 2. IGNORE COLLISIONS between this ghost and the Mimic's main body
        if (mimicTransform != null)
        {
            Collider mimicMainCollider = mimicTransform.GetComponent<Collider>();
            if (mimicMainCollider != null)
            {
                Physics.IgnoreCollision(ghostCollider, mimicMainCollider, true);
            }
        }
    }

    void Update()
    {
        // 3. Smoothly follow the Mimic around the map
        if (mimicTransform != null)
        {
            transform.position = mimicTransform.position;
            transform.rotation = mimicTransform.rotation;
        }
        else
        {
            // If the Mimic was deleted, destroy this ghost too!
            Destroy(gameObject);
        }
    }
}