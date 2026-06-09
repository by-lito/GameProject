using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet instance; // Esto permite que otros scripts te encuentren f�cil

    [Header("RUN (Se pierde al morir)")]
    public int angelDust = 0;

    [Header("LOBBY (Permanente)")]
    public int memories = 0;
    public int potionFragments = 0;
    public int permanentMoney = 0; // Lo que ganas al final del nivel


    // Asegura de que solo haya una billetera en el juego y que se registre a s� misma al iniciar el juego.
    void Awake()
    {
        instance = this; // Al empezar, este script se registra a s� mismo
    }

    // Esto se llama cuando matas al tri�ngulo rojo
    public void AddAngelDust(int amount)
    {
        angelDust += amount;
        StatsTracker.Instance?.AddMoney(amount);
        Debug.Log("�Polvo de �ngel recogido! Total: " + angelDust);
    }

    // Llama a esto cuando el jugador muera o reinicie la partida
    public void ResetRun()
    {
        angelDust = 0;
        Debug.Log("Run terminada. El Polvo de �ngel se ha esfumado.");
    }

    // Esto se llama solo cuando tocas la meta/final del nivel
    public void AddEndLevelReward(int amount)
    {
        permanentMoney += amount;
        Debug.Log("�Nivel superado! Dinero para el Lobby: " + permanentMoney);
    }

    // Para la tienda de la RUN
    // 1. Solo comprueba si tiene el dinero (sin restar nada)
    public bool CanAfford(int cost)
    {
        return angelDust >= cost;
    }

    // 2. Usar este m�todo para gastar polvo de �ngel.
    public bool SpendDust(int cost)
    {
        if (angelDust >= cost)
        {
            // Restamos el coste al total de polvo de �ngel
            angelDust -= cost;
            return true;
        }
        return false;
    }

    //Para la tienda del LOBBY
    // M�todo para gastar el dinero que no se pierde al morir (Lobby/Nexo)
    public void SpendPermanentMoney(int amount)
    {
        // Restamos la cantidad al total permanente
        permanentMoney -= amount;
        Debug.Log("Has gastado dinero permanente. Quedan: " + permanentMoney);
    }
}