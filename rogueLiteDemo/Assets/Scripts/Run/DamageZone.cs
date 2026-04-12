using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador ha entrado en una zona de daño");
        }
    }
}