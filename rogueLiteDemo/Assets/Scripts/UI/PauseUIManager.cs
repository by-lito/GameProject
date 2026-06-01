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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Forzamos la visibilidad según la escena en la que estemos
        if (scene.name == "MainMenu")
        {
            pauseButtonGO.SetActive(false);
            pausePanelGO.SetActive(false);
        }
        else
        {
            // En cualquier escena de juego, el botón PAUSE estará activo SÍ O SÍ
            pauseButtonGO.SetActive(true);
            pausePanelGO.SetActive(false);
        }
    }

    // ── MÉTODOS PARA LOS EVENTOS ONCLICK EN UNITY ───────────────────────

    public void TriggerPause()
    {
        // Activamos el panel visual inmediatamente al pulsar
        pausePanelGO.SetActive(true);
        pauseButtonGO.SetActive(false);

        // Intentamos congelar el tiempo del juego
        Time.timeScale = 0f;

        // Si vuestro GameManager existe, le avisamos por si acaso
        if (GameManager.Instance != null)
        {
            // Forzamos el estado a Paused directamente para que no ponga pegas
            GameManager.Instance.Pause();
        }
    }

    public void TriggerResume()
    {
        // Restauramos la interfaz
        pausePanelGO.SetActive(false);
        pauseButtonGO.SetActive(true);

        // Descongelamos el tiempo
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Resume();
        }
    }

    public void TriggerExit()
    {
        Time.timeScale = 1f; // Aseguramos que el tiempo vuelve a la normalidad
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMenu();
        }
        SceneManager.LoadScene("MainMenu");
    }
}