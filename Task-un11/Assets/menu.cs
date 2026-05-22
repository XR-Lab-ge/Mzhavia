using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading levels

public class MenuActions : MonoBehaviour
{


    [Header("Scene Settings")]
    [Tooltip("The EXACT name of your gameplay scene as it appears in your Assets folder")]
    public string gameSceneName = "GameScene";
    // This function will run when you click your UI button
    public void RestartGame()
    {
        // 1. SAFETY: Reset time back to normal speed so the game isn't frozen on load!
        Time.timeScale = 1f;

        // 2. Get the name of your currently active scene
        string activeSceneName = SceneManager.GetActiveScene().name;

        // 3. Reload it from scratch
        SceneManager.LoadScene(activeSceneName);

        Debug.Log("🔄 Scene reloaded successfully!");
    }

    public void ExitMenu()
    {
        Time.timeScale = 1f;

        // Load the specified gameplay level
        SceneManager.LoadScene(gameSceneName);

        Debug.Log($"🎮 Loading scene: {gameSceneName}");
    }
}
