using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifeTime = 5f;

    private Rigidbody rb;
    private float spawnTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Forzamos que no caiga

        // Disparar hacia adelante
        rb.linearVelocity = transform.forward * speed;

        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Ignorar al jugador siempre
        if (other.CompareTag("Player")) return;

        // 2. Si es un enemigo, daño y muerte inmediata
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // 3. SI CHOCA CON EL SUELO O PAREDES
        if (other.CompareTag("Environment") || other.CompareTag("Walls"))
        {
            // ESCUDO: Si ha pasado menos de 0.2 segundos, IGNOTAMOS el suelo
            if (Time.time < spawnTime + 0.2f)
            {
                return;
            }

            // Si ya está lejos, entonces sí se destruye
            Destroy(gameObject);
        }
    }
}