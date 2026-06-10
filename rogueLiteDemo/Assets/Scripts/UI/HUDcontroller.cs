using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Persists across scenes. Subscribes to PlayerWallet.OnAngelDustChanged
/// so AngelDust updates automatically without polling.
/// PlayerController.ActualizarHUDLocal() already handles health + coins.
/// </summary>
public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("Componentes de UI")]
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private TextMeshProUGUI monedasText;    // lobby coins
    [SerializeField] private TextMeshProUGUI polvoText;      // AngelDust (run)
    [SerializeField] private TextMeshProUGUI salasText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeWallet();
    }

    private void Start()
    {
        SubscribeWallet();
    }

    // Re-subscribe when scene reloads (PlayerWallet persists, but Start() already ran)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ToggleHUDVisibility(scene.name != "MainMenu" && scene.name != "Boot");
        SubscribeWallet();
    }

    // ── Wallet subscription ───────────────────────────────────────────

    private void SubscribeWallet()
    {
        if (PlayerWallet.instance == null) return;
        // Unsubscribe first to avoid double-subscription on scene reload
        PlayerWallet.instance.OnAngelDustChanged -= SetPolvo;
        PlayerWallet.instance.OnAngelDustChanged += SetPolvo;
        // Refresh immediately
        SetPolvo(PlayerWallet.instance.angelDust);
    }

    private void UnsubscribeWallet()
    {
        if (PlayerWallet.instance != null)
            PlayerWallet.instance.OnAngelDustChanged -= SetPolvo;
    }

    // ── Public setters (called by PlayerController.ActualizarHUDLocal) ─

    private int lastMonedas = -1;
    private int lastSalas = -1;

    private void Update()
    {
        if (PlayerWallet.instance != null && PlayerWallet.instance.angelDust != lastMonedas)
        {
            lastMonedas = PlayerWallet.instance.angelDust;
            SetMonedas(lastMonedas);
        }

        if (RunManager.Instance != null && RunManager.Instance.RoomsCompletedThisRun != lastSalas)
        {
            lastSalas = RunManager.Instance.RoomsCompletedThisRun;
            SetSalas(lastSalas);
        }
    }

    public void SetVidas(float saludActual)
    {
        if (barraDeVida != null) barraDeVida.value = saludActual;
    }

    public void SetMonedas(int monedas)
    {
        if (monedasText != null) monedasText.text = "Monedas: " + monedas;
    }

    public void SetSalas(int salas)
    {
        if (salasText != null) salasText.text = "Salas: " + salas;
    }

    // Called automatically via OnAngelDustChanged event
    private void SetPolvo(int polvo)
    {
        if (polvoText != null) polvoText.text = "✦ " + polvo;
    }

    // ── Visibility ────────────────────────────────────────────────────

    private void ToggleHUDVisibility(bool visible)
    {
        Canvas c = GetComponent<Canvas>();
        if (c != null) c.enabled = visible;
    }
}