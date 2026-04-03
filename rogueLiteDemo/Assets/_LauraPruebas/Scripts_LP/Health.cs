using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para volver al lobby si "mueres"

public class Health : MonoBehaviour
{
    [Header("Configuración de Vitalidad")]
    public float currentVitality = 0f; // Empieza en 0%
    public float maxVitality = 100f;   // El límite para "perder" (resucitar)

    void Start()
    {
        currentVitality = 0f;
    }

    // Los ENEMIGOS llaman a esto (La curan = Malo para la protagonista)
    public void ReceiveHealing(float amount)
    {
        currentVitality += amount;
        Debug.Log("¡Cuidado! Te están curando. Vitalidad actual: " + currentVitality + "%");

        if (currentVitality >= maxVitality)
        {
            Resurrected(); // Pierde y vuelve al lobby
        }
    }

    // La protagonista llama a esto al atacar (Se hace daño = Bueno para ella)
    //Podemos quitarlo si no queremos que se haga daño a sí misma, pero lo dejamos por si queremos hacer un "modo difíci"

    public void SelfDamage(float amount)
    {
        currentVitality -= amount;
        if (currentVitality < 0) currentVitality = 0; // No puede bajar de 0

        Debug.Log("Te has hecho daño. Vitalidad actual: " + currentVitality + "%");

        if (currentVitality <= 0)
        {
            // Aquí iría la lógica de "Ganar nivel" o "Morir con éxito"
            Debug.Log("¡Objetivo conseguido! Has alcanzado el 0%.");
        }
    }

    void Resurrected()
    {
        Debug.Log("Has resucitado por completo... Volviendo al lobby.");
        // SceneManager.LoadScene("Lobby"); // Descomentar esto cuando David tenga el Lobby creado
        Destroy(gameObject);
    }
}