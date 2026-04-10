using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    public float maxHealth = 30f; // Pusimos 30 para que muriera de 3 golpes (10 cada uno)
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Feedback en consola para saber que el ataque funciona
        Debug.Log("¡Enemigo golpeado! Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El enemigo ha sido destruido.");
        // Antes de desaparecer, le damos el polvo al jugador
        if (PlayerWallet.instance != null)
        {
            PlayerWallet.instance.AddAngelDust(15); //La cantidad de polvo que da el enemigo, podemos cambiarlo
        }
        // Destruye el objeto del enemigo
        Destroy(gameObject);
    }
}