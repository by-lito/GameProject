using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance { get; private set; }

    [Header("UI GameObjects")]
    [SerializeField] private GameObject pauseButtonGO; // El botón PAUSE de la esquina
    [SerializeField] private GameObject pausePanelGO;  // El panel oscuro con Reanudar/Salir

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Nos suscribimos a la carga de escenas para ocultar o mostrar el botón PAUSE automáticamente
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // En el menú principal no debe haber botón de pausa por defecto
        if (scene.name == "MainMenu")
        {
            if (pauseButtonGO != null) pauseButtonGO.SetActive(false);
            if (pausePanelGO != null) pausePanelGO.SetActive(false);
        }
        else
        {
            if (pauseButtonGO != null) pauseButtonGO.SetActive(true);
            if (pausePanelGO != null) pausePanelGO.SetActive(false);
        }
    }

    // ── MÉTODOS PARA LOS EVENTOS ONCLICK EN UNITY ───────────────────────

    // 1. Llamar desde el botón PAUSE de la esquina
    public void TriggerPause()
    {
        if (pausePanelGO != null) pausePanelGO.SetActive(true);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(false);

        Time.timeScale = 0f; // Congela por completo la física y el movimiento del juego
    }

    // 2. Llamar desde el botón REANUDAR del panel de pausa
    public void TriggerResume()
    {
        if (pausePanelGO != null) pausePanelGO.SetActive(false);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(true);

        Time.timeScale = 1f; // Devuelve el tiempo a la normalidad
    }

    // 3. Llamar desde el botón GUARDAR Y SALIR (Vuelve al menú principal)
    public void TriggerExit()
    {
        // Buscamos al jugador que está actualmente en el nivel
        PlayerController jugadorActual = FindAnyObjectByType<PlayerController>();

        // Si el jugador existe en la escena, guardamos sus datos locales en Firebase antes de salir
        if (jugadorActual != null && FirebaseSaveHandler.Instance != null)
        {
            Debug.Log("Guardando partida local en Firestore antes de salir...");

            PlayerSaveData datosA_Guardar = new PlayerSaveData
            {
                currentHealth = (int)jugadorActual.currentHealth,
                coins = jugadorActual.coins,
                roomsCompleted = jugadorActual.roomsCompleted,
                maxHealth = 100 // O la variable de salud máxima que tengáis
            };

            // Llamamos a vuestro método de Firebase para sobrescribir el documento en la nube
            // Nota: Cambia "UpdateHUD" por el método de guardado (SetAsync) que tengáis en FirebaseSaveHandler
            // Ejemplo si tenéis: FirebaseSaveHandler.Instance.SavePlayerData(datosA_Guardar);
        }

        Time.timeScale = 1f; // IMPORTANTE: Descongelamos el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu");
    }
}