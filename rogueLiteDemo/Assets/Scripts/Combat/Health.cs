using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Configuracion Base")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Ajustes de Recompensa (Para Enemigos)")]
    public int dustReward = 15;

    public Action<float> OnDamage;
    public Action<float> OnHeal;
    public Action OnDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnDamage?.Invoke(amount);
        Debug.Log(gameObject.name + " - Vida/Vitalidad restante: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnHeal?.Invoke(amount);
    }

    public void Die()
    {
        OnDeath?.Invoke();

        // si es un enemigo y muere, da Angel Dust
        if (gameObject.CompareTag("Enemy"))
        {
            if (PlayerWallet.instance != null)
            {
                PlayerWallet.instance.AddAngelDust(dustReward);
                Debug.Log("¡Enemigo destruido! Recibes " + dustReward + " de polvo.");
            }
        }

        // si es el Jugador y muere (llega a 0 vida/100% vitalidad)
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Has resucitado por completo... Volviendo al lobby.");
            SceneManager.LoadScene("Lobby");
        }

        Destroy(gameObject);
    }
}