/*using UnityEngine;

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
        // Destruye el objeto del enemigo (el Triángulo)
        Destroy(gameObject);
    }
}*/