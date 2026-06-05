using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Punto de entrada de la aplicación. Vive en la escena Boot (índice 0).
/// Inicializa los sistemas core en orden y luego carga el menú principal.
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

    // ── Inicialización ───────────────────────────────────────────────

    private void InitializeSystems()
    {
        EnsureGameManager();
        EnsureWallet();
        Debug.Log("[Bootstrap] Core systems initialized.");
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance != null) return;

        if (gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }
        else
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.LogWarning("[Bootstrap] GameManager prefab not assigned — created at runtime.");
        }
    }

    // Crea UNA billetera persistente para toda la sesión.
    // El Angel Dust y el dinero permanente sobreviven a los cambios de escena.
    // IMPORTANTE: no coloques PlayerWallet en ninguna escena; de esto se encarga el Boot.
    private void EnsureWallet()
    {
        if (PlayerWallet.instance != null) return;

        GameObject go = new GameObject("PlayerWallet");
        go.AddComponent<PlayerWallet>();
        DontDestroyOnLoad(go);
        Debug.Log("[Bootstrap] PlayerWallet persistente creada.");
    }

    // ── Carga de escena ──────────────────────────────────────────────

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