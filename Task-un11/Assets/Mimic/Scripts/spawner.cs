using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Required for analyzing live NavMeshAgents

[RequireComponent(typeof(BoxCollider))]
public class MimicSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject mimicPrefab;
    public int initialSpawnCount = 3;
    public int extraMimicsPerWave = 2;

    [Tooltip("How often (in seconds) the script scans the scene. Lower = more instant wave drops.")]
    public float scanningInterval = 0.3f;

    [Header("Live Metrics Tracker")]
    // Globally visible counters that track stats across scripts
    public static int TotalKilledMimics = 0;
    public static int TotalSpawnedMimics = 0;

    private BoxCollider spawnArea;
    private int currentWave = 0;
    private int nextWaveSpawnCount;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        spawnArea.isTrigger = true;

        nextWaveSpawnCount = initialSpawnCount;
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            currentWave++;

            // 1. Drop the new wave of enemies onto the map
            SpawnWave(nextWaveSpawnCount);

            Debug.Log($"<color=cyan><b>WAVE {currentWave} DEPLOYED!</b></color> Spawning {nextWaveSpawnCount} targets. | Total Spawned Ever: {TotalSpawnedMimics}");

            // 2. Scale the size calculation up for the *next* wave cycle
            nextWaveSpawnCount += extraMimicsPerWave;

            // 3. THE WAITING SYSTEM: Pause right here until ONLY ONE mimic remains in the entire game
            yield return StartCoroutine(WaitForLastMimicRemaining());

            Debug.Log($"<color=orange><b>ONLY ONE LEFT ALIVE!</b></color> Unleashing Wave {currentWave + 1} automatically!");

            // Give a tiny 0.5-second cinematic delay before the next wave hits the ground
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WaitForLastMimicRemaining()
    {
        bool multipleStillAlive = true;

        while (multipleStillAlive)
        {
            // Find all active NavMeshAgents (your Mimics) left alive in your level hierarchy
            NavMeshAgent[] aliveMimics = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);

            // CHANGED: Instead of checking for 0, we break the loop and spawn the next wave if count is 1 or less
            if (aliveMimics.Length <= 1)
            {
                multipleStillAlive = false;
            }
            else
            {
                // More than one mimic is still active. Stay on standby.
                yield return new WaitForSeconds(scanningInterval);
            }
        }
    }

    void SpawnWave(int count)
    {
        if (mimicPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            // Pick a clean random point directly inside your box boundaries
            Vector3 center = spawnArea.bounds.center;
            Vector3 extents = spawnArea.bounds.extents;

            Vector3 randomPos = new Vector3(
                Random.Range(center.x - extents.x, center.x + extents.x),
                Random.Range(center.y - extents.y, center.y + extents.y),
                Random.Range(center.z - extents.z, center.z + extents.z)
            );

            Instantiate(mimicPrefab, randomPos, Quaternion.identity);

            // TRACKING: Increment our total spawn metric for every single instantiation
            TotalSpawnedMimics++;
        }
    }
}