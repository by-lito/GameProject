using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    public TextMeshProUGUI vidasText;
    public TextMeshProUGUI monedasText;
    public TextMeshProUGUI salasText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Esto hace que el HUD no desaparezca al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Evita que se duplique si vuelves a MainMenu
        }
    }

    // Métodos para actualizar valores
    public void SetVidas(int v)
    {
        vidasText.text = "Vidas: " + v;
    }

    public void SetMonedas(int m)
    {
        monedasText.text = "Monedas: " + m;
    }

    public void SetSalas(int s)
    {
        salasText.text = "Salas: " + s;
    }
}