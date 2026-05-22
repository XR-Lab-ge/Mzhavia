using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;        // ADDED: For standard UI Text
using TMPro;                // ADDED: For TextMeshPro Text

public class DistanceClickDestroyer : MonoBehaviour
{
    [Header("Settings")]
    public Camera mainCamera;

    [Tooltip("How close your mouse click needs to be to a Mimic to register a hit")]
    public float clickSensitivity = 3.0f;

    [Tooltip("How many clicks it takes to destroy a Mimic")]
    public int hitsRequired = 5;

    [Header("UI Display Settings")]
    [Tooltip("Drag your TextMeshPro component here if you use TMPro")]
    public TextMeshProUGUI tmproText;

    [Tooltip("Drag your legacy standard UI Text component here if you use standard UI")]
    public Text legacyText;

    [Tooltip("Text prefix before the score number")]
    public string labelPrefix = "Kills: ";

    private Dictionary<GameObject, int> mimicHitTracker = new Dictionary<GameObject, int>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;


        MimicSpawner.TotalKilledMimics = 0;

        // Update the text right at the start so it doesn't show default placeholder text
        UpdateScoreUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessClickDamage();
        }
    }

    void ProcessClickDamage()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        NavMeshAgent[] allMimics = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);

        GameObject closestMimic = null;
        float closestDistance = float.MaxValue;

        foreach (NavMeshAgent mimic in allMimics)
        {
            if (mimic == null) continue;
            GameObject mimicObj = mimic.gameObject;

            float distanceToRay = Vector3.Cross(ray.direction, mimicObj.transform.position - ray.origin).magnitude;

            if (distanceToRay <= clickSensitivity && distanceToRay < closestDistance)
            {
                closestDistance = distanceToRay;
                closestMimic = mimicObj;
            }
        }

        if (closestMimic != null)
        {
            if (!mimicHitTracker.ContainsKey(closestMimic))
            {
                mimicHitTracker.Add(closestMimic, 0);
            }

            mimicHitTracker[closestMimic]++;
            int currentHits = mimicHitTracker[closestMimic];

            Debug.Log($"🎯 Hit registered on {closestMimic.name}! ({currentHits}/{hitsRequired})");

            if (currentHits >= hitsRequired)
            {
                MimicSpawner.TotalKilledMimics++;
                Debug.Log($"💥 {closestMimic.name} Destroyed Instantly! Total Score: {MimicSpawner.TotalKilledMimics}");

                // ADDED: Call our UI update function right here when a mimic dies!
                UpdateScoreUI();

                mimicHitTracker.Remove(closestMimic);
                Destroy(closestMimic);
            }
        }

        CleanTrackerData();
    }

    // ADDED: Simple function that refreshes the text on your screen
    void UpdateScoreUI()
    {
        string finalizedText = labelPrefix + MimicSpawner.TotalKilledMimics.ToString();

        if (tmproText != null)
        {
            tmproText.text = finalizedText;
        }
        else if (legacyText != null)
        {
            legacyText.text = finalizedText;
        }
    }

    void CleanTrackerData()
    {
        List<GameObject> keysToRemove = new List<GameObject>();
        foreach (var key in mimicHitTracker.Keys)
        {
            if (key == null) keysToRemove.Add(key);
        }
        foreach (var key in keysToRemove)
        {
            mimicHitTracker.Remove(key);
        }
    }
}