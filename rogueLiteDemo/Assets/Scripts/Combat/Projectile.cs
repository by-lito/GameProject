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
        if (rb != null) rb.useGravity = false;
    }

    void Start()
    {
        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);
    }

    public void SetupDirection(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 velocityDirection = new Vector3(direction.x, 0f, direction.y).normalized;
            rb.linearVelocity = velocityDirection * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nunca chocar con el propio jugador.
        if (other.CompareTag("Player")) return;

        // Enemigo: daño y destrucción.
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Health>(out Health health))
                health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Environment"))
        {
            if (Time.time < spawnTime + 0.2f) return;
            Destroy(gameObject);
        }
    }
}