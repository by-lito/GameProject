using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Entry point of the application. Lives in the Boot scene (index 0).
/// Initializes core systems in the correct order, then loads the Main Menu.
/// Add this to a single GameObject in the Boot scene.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("Boot Settings")]
    [SerializeField] private string firstScene = "MainMenu";

    [Header("Core Prefabs (optional — assign if not in scene)")]
    [SerializeField] private GameManager gameManagerPrefab;

    void Awake()
    {
        InitializeSystems();
    }

    void Start()
    {
        LoadFirstScene();
    }

    // ── Initialization ───────────────────────────────────────────────

    private void InitializeSystems()
    {
        EnsureGameManager();

        // Add additional system initialization here as the project grows
        // e.g.: EnsureAudioManager(), EnsureInputManager(), etc.

        Debug.Log("[Bootstrap] Core systems initialized.");
    }

    private void EnsureGameManager()
    {
        // If GameManager already exists (e.g. hot reload), skip
        if (GameManager.Instance != null) return;

        if (gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }
        else
        {
            // Create a minimal GameManager at runtime if no prefab assigned
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.LogWarning("[Bootstrap] GameManager prefab not assigned — created at runtime.");
        }
    }

    // ── Scene loading ────────────────────────────────────────────────

    private void LoadFirstScene()
    {
        if (string.IsNullOrEmpty(firstScene))
        {
            Debug.LogError("[Bootstrap] firstScene name is empty. Set it in the Inspector.");
            return;
        }

        SceneManager.LoadScene(firstScene);
    }
}