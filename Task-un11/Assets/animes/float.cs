using UnityEngine;

public class FloatingDecoration : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("The maximum distance the object will drift up and down from its starting point.")]
    public float floatAmplitude = 0.5f;

    [Tooltip("How fast the object cycles up and down. Higher = faster bobbing.")]
    public float floatSpeed = 2.0f;

    [Header("3D Anti-Gravity Rotation")]
    [Tooltip("Vertical rotation speed (spinning on the spot).")]
    public float verticalRotationSpeed = 20f;

    [Tooltip("Horizontal tumbling speed (rolling and pitching forward/sideways). Set to 0 to disable.")]
    public float horizontalTumbleSpeed = 15f;

    private Vector3 startPosition;
    private float randomTimeOffset;
    private Vector3 randomTumbleAxis;

    void Start()
    {
        startPosition = transform.position;

        // Desynchronizes multiple objects so they don't look like robots moving in perfect sync
        randomTimeOffset = Random.Range(0f, 50f);

        // Generates a completely unique horizontal tumbling axis for this specific object
        // This makes sure different decorations tumble in completely different directions!
        randomTumbleAxis = new Vector3(
            Random.Range(-1f, 1f),
            0f, // Keeping Y out of this variable since verticalRotationSpeed handles it cleanly
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void Update()
    {
        float customTime = Time.time + randomTimeOffset;

        // 1. Math smooth sine wave calculation for height shifting
        float newY = startPosition.y + (Mathf.Sin(customTime * floatSpeed) * floatAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 2. Continuous Vertical Spin (around the Y axis)
        if (verticalRotationSpeed > 0)
        {
            transform.Rotate(Vector3.up, verticalRotationSpeed * Time.deltaTime, Space.World);
        }

        // 3. Continuous Horizontal Tumble (Pitching and Rolling over its side)
        if (horizontalTumbleSpeed > 0)
        {
            // Rotates the object around our unique custom horizontal axis over time
            transform.Rotate(randomTumbleAxis, horizontalTumbleSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 basePoint = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(basePoint + Vector3.down * floatAmplitude, basePoint + Vector3.up * floatAmplitude);
        Gizmos.DrawWireSphere(basePoint + Vector3.up * floatAmplitude, 0.1f);
        Gizmos.DrawWireSphere(basePoint + Vector3.down * floatAmplitude, 0.1f);
    }
}