using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your existing UI Slider here from the Hierarchy")]
    public Slider healthSlider;

    [Header("Game Over Settings")]
    [Tooltip("Drag your Game Over Panel / Screen object here from the Hierarchy")]
    public GameObject gameOverPanel;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Distance-Based Attack Settings")]
    [Tooltip("How close a mimic has to get (in meters) to damage the player")]
    public float attackRange = 1.5f;
    public float damagePerTouch = 10f;
    public float damageCooldown = 0.5f;
    private float nextDamageTime;

    private bool isDead = false;

    void Start()
    {
        // Ensure the game runs at normal speed when starting/reloading
        Time.timeScale = 1f;

        currentHealth = maxHealth;

        // Sync slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Automatically turn OFF the Game Over screen when the match starts
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead) return;
        if (Time.time < nextDamageTime) return;

        NavMeshAgent[] aliveMimics = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);

        foreach (NavMeshAgent mimic in aliveMimics)
        {
            if (mimic == null) continue;

            float distanceToPlayer = Vector3.Distance(transform.position, mimic.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                TakeDamage();
                break;
            }
        }
    }

    void TakeDamage()
    {
        currentHealth -= damagePerTouch;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"💥 Hit! Health: {currentHealth}");

        if (currentHealth <= 0f)
        {
            PlayerDeath();
        }
        else
        {
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    void PlayerDeath()
    {
        isDead = true;

        // 1. Freeze the game clock instantly
        Time.timeScale = 0f;

        // 2. Turn on your Game Over screen overlay
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 3. FIXED: Release and unlock the cursor from gun aiming state!
        Cursor.lockState = CursorLockMode.None; // Stops locking mouse to the center of the screen
        Cursor.visible = true;                  // Makes the cursor visible again

        Debug.Log("💀 GAME OVER. Screen revealed, game paused, and cursor unlocked!");
    }
}