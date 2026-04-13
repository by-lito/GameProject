/*using UnityEngine;

public class EnemyHealer : MonoBehaviour
{
    public float healAmount = 10f; // Cantidad de curación que el enemigo le da al Player, podemos cambiarlo

    // Se activa cuando el Player toca al enemigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Buscamos el componente Health en el Player y le subimos la barra
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.ReceiveHealing(healAmount);
            }
        }
    }
}*/