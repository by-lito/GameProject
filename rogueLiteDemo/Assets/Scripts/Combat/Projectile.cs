using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 2f; 

    void Start()
    {
        // Se destruye solo a los 2 segundos para no llenar la memoria
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // En 3D usamos Vector3.forward o transform.forward 
        // para que la bala vaya hacia donde apunta el "cañón"
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Cambiamos a OnTriggerEnter (3D) para que detecte colliders en el mapa 3D
    private void OnTriggerEnter(Collider collision)
    {
        // Si choca con un enemigo
        if (collision.CompareTag("Enemy"))
        {
            // Usamos la clase base Health de Ángel (Arquitectura Modular)
            if (collision.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }
            
            Destroy(gameObject); // La bala desaparece al chocar
        }
        
        // Opcional: Que la bala desaparezca si choca con una pared (Escenario)
        if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}