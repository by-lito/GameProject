using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifeTime = 5f;

    private Rigidbody rb;
    private float spawnTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Forzamos que no caiga
        }
    }

    void Start()
    {
        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// [NUEVO] Recibe la dirección en la que está mirando Aurora y le aplica velocidad física real.
    /// </summary>
    public void SetupDirection(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Convertimos la dirección 2D del Animator (X, Y) al movimiento 3D de tu mapa (X, 0, Z)
            Vector3 velocityDirection = new Vector3(direction.x, 0f, direction.y).normalized;

            rb.linearVelocity = velocityDirection * speed;
        }
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
            // ESCUDO: Si ha pasado menos de 0.2 segundos, IGNORAMOS el suelo
            if (Time.time < spawnTime + 0.2f)
            {
                return;
            }

            // Si ya está lejos, entonces sí se destruye
            Destroy(gameObject);
        }
    }
}