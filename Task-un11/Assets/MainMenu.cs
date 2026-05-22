using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class MainMenuActions : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The EXACT name of your gameplay scene as it appears in your Assets folder")]
    public string gameSceneName = "GameScene";

    // FUNCTION 1: Run this to load into your gameplay map
    public void StartGame()
    {
        // Safety: Always restore time scale in case the previous match ended on a pause
        Time.timeScale = 1f;

        // Load the specified gameplay level
        SceneManager.LoadScene(gameSceneName);

        Debug.Log($"🎮 Loading scene: {gameSceneName}");
    }

    // FUNCTION 2: Run this to completely close the game application
    public void ExitGame()
    {
        Debug.Log("🚪 Exit Button Pressed! Closing game...");

        // Closes the built standalone executable (.exe / app build)
        Application.Quit();

        // If you are testing inside the Unity Engine, this line stops the editor playmode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}