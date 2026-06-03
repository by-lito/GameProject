using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Añade esta línea para controlar escenas
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("Componentes de UI")]
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private TextMeshProUGUI monedasText;
    [SerializeField] private TextMeshProUGUI salasText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Nos aseguramos de que sea persistente

        // Nos suscribimos al evento de carga de escenas de Unity
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Limpiamos el evento al destruirse para evitar errores de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si estamos en el Menú Principal, el HUD se esconde por completo
        if (scene.name == "MainMenu")
        {
            ToggleHUDVisibility(false);
        }
        else
        {
            // En cualquier otra escena de juego (Lobby, Combate, etc.), el HUD se enciende solo
            ToggleHUDVisibility(true);
        }
    }

    private void ToggleHUDVisibility(bool visible)
    {
        // Desactivamos el componente Canvas para ocultar los elementos de golpe
        // sin romper las referencias de los scripts que intenten actualizar los textos
        Canvas miCanvas = GetComponent<Canvas>();
        if (miCanvas != null)
        {
            miCanvas.enabled = visible;
        }
    }

    public void SetVidas(float saludActual)
    {
        if (barraDeVida != null)
        {
            barraDeVida.value = saludActual; // La barra se moverá sola del 0 al 100
        }
    }

    public void SetMonedas(int monedas)
    {
        if (monedasText != null) monedasText.text = "Monedas: " + monedas;
    }

    public void SetSalas(int salas)
    {
        if (salasText != null) salasText.text = "Salas: " + salas;
    }
}