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
        Debug.Log("SET VIDAS: " + v);
        vidasText.text = "Vidas: " + v;
    }

    public void SetMonedas(int m)
    {
        Debug.Log("SET MONEDAS: " + m);
        monedasText.text = "Monedas: " + m;
    }

    public void SetSalas(int s)
    {
        Debug.Log("SET SALAS: " + s);
        salasText.text = "Salas: " + s;
    }
}