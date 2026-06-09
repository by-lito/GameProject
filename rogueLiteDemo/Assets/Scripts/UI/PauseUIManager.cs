using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;  

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance { get; private set; }

    [Header("UI GameObjects")]
    [SerializeField] private GameObject pauseButtonGO; 
    [SerializeField] private GameObject pausePanelGO;  

    private bool isPaused = false;
    private bool inGameplay = false;  

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        inGameplay = SceneManager.GetActiveScene().name != "MainMenu";
        isPaused = false;
        if (pausePanelGO != null) pausePanelGO.SetActive(false);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(inGameplay);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inGameplay = scene.name != "MainMenu";

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanelGO != null) pausePanelGO.SetActive(false);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(inGameplay);
    }

    private void Update()
    {
        if (inGameplay && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) TriggerResume();
        else          TriggerPause();
    }

    public void TriggerPause()
    {
        if (!inGameplay) return;
        isPaused = true;
        if (pausePanelGO != null) pausePanelGO.SetActive(true);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(false);
        Time.timeScale = 0f;
    }

    public void TriggerResume()
    {
        isPaused = false;
        if (pausePanelGO != null) pausePanelGO.SetActive(false);
        if (pauseButtonGO != null) pauseButtonGO.SetActive(true);
        Time.timeScale = 1f;
    }

    public void TriggerExit()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}