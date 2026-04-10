using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet instance; // Esto permite que otros scripts te encuentren fácil

    [Header("RUN (Se pierde al morir)")]
    public int angelDust = 0;

    [Header("LOBBY (Permanente)")]
    public int memories = 0;
    public int potionFragments = 0;
    public int permanentMoney = 0; // Lo que ganas al final del nivel


    // Asegura de que solo haya una billetera en el juego y que se registre a sí misma al iniciar el juego.
    void Awake()
    {
        instance = this; // Al empezar, este script se registra a sí mismo
    }

    // Esto se llama cuando matas al triángulo rojo
    public void AddAngelDust(int amount)
    {
        angelDust += amount;
        Debug.Log("¡Polvo de Ángel recogido! Total: " + angelDust);
    }

    // Llama a esto cuando el jugador muera o reinicie la partida
    public void ResetRun()
    {
        angelDust = 0;
        Debug.Log("Run terminada. El Polvo de Ángel se ha esfumado.");
    }

    // Esto se llama solo cuando tocas la meta/final del nivel
    public void AddEndLevelReward(int amount)
    {
        permanentMoney += amount;
        Debug.Log("¡Nivel superado! Dinero para el Lobby: " + permanentMoney);
    }

    // Para la tienda del nivel
    public bool CanSpendDust(int cost)
    {
        if (angelDust >= cost)
        {
            angelDust -= cost;
            return true;
        }
        return false;
    }
}