using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance { get; private set; }

    [Header("UI GameObjects")]
    [SerializeField] private GameObject pauseButtonGO; // Arrastra aquí el PauseButton
    [SerializeField] private GameObject pausePanelGO;  // Arrastra aquí el PausePanel

    private void Awake()
    {
        // Volvemos el Canvas persistente para que nos sirva en todas las salas del juego
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void Start()
    {
        // Nos suscribimos al evento de vuestro GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
        }

        CheckSceneVisibility(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneVisibility(scene.name);
    }

    private void CheckSceneVisibility(string sceneName)
    {
        // Si volvemos al menú principal por el botón de salir, se oculta todo automáticamente
        if (sceneName == "MainMenu")
        {
            pauseButtonGO.SetActive(false);
            pausePanelGO.SetActive(false);
        }
        else
        {
            // En cualquier otra pantalla del juego (Lobby_3D, salas), el botón de la esquina aparece
            pauseButtonGO.SetActive(true);
            pausePanelGO.SetActive(false);
        }
    }

    // Este método se ejecuta solo cuando nuestro GameManager cambia de estado interno
    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Paused)
        {
            pausePanelGO.SetActive(true);   // Muestra el panel con opciones
            pauseButtonGO.SetActive(false); // Oculta el botón de la esquina
        }
        else if (newState == GameManager.GameState.Playing)
        {
            pausePanelGO.SetActive(false);  // Oculta el panel
            pauseButtonGO.SetActive(true);  // Muestra el botón de la esquina
        }
        else
        {
            // Si el estado es Menu o Dead, apagamos la UI de pausa por completo
            pausePanelGO.SetActive(false);
            pauseButtonGO.SetActive(false);
        }
    }

    // ── MÉTODOS PARA LOS EVENTOS ONCLICK EN UNITY ───────────────────────

    public void TriggerPause()
    {
        Debug.Log("¡El botón de pausa se ha pulsado correctamente!");
        if (GameManager.Instance != null) GameManager.Instance.Pause();
    }

    public void TriggerResume()
    {
        if (GameManager.Instance != null) GameManager.Instance.Resume();
    }

    public void TriggerExit()
    {
        if (GameManager.Instance != null) GameManager.Instance.GoToMenu();
        SceneManager.LoadScene("MainMenu");
    }
}