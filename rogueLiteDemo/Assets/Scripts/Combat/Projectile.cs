using UnityEngine;

/// <summary>
/// Gestiona el comportamiento físico, el movimiento lineal y la detección de colisiones
/// de los proyectiles disparados por el jugador en un entorno 3D.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Ajustes de Proyectil")]
    [Tooltip("Velocidad de desplazamiento del proyectil.")]
    public float speed = 15f;

    [Tooltip("Cantidad de daño que infligirá al impactar con un enemigo.")]
    public int damage = 10;

    [Tooltip("Tiempo en segundos antes de que el proyectil se destruya automáticamente.")]
    public float lifeTime = 2f;

    private Rigidbody rb;

    /// <summary>
    /// Inicializa los componentes físicos y aplica la fuerza inicial.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Configuramos el Rigidbody para que no se vea afectado por la gravedad
        // y se mueva de forma lineal constante.
        rb.useGravity = false;

        // Aplicamos velocidad en la dirección 'forward' del proyectil.
        // El proyectil viajará hacia donde apunte el eje Z (azul) del ShootPoint.
        rb.linearVelocity = transform.forward * speed;

        // Gestión de memoria: eliminamos el objeto tras su tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Maneja la detección de colisiones mediante triggers para aplicar daño o destruir el proyectil.
    /// </summary>
    /// <param name="collision">El Collider del objeto con el que se ha producido el impacto.</param>
    private void OnTriggerEnter(Collider collision)
    {
        // Evitamos que el proyectil se destruya al colisionar con el propio jugador durante el spawn.
        if (collision.CompareTag("Player")) return;

        // Verificamos si el objeto impactado es un enemigo.
        if (collision.CompareTag("Enemy") || collision.CompareTag("Enemies"))
        {
            // Intentamos obtener el componente Health para aplicar daño.
            if (collision.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }

            // El proyectil se destruye tras impactar con un objetivo válido.
            Destroy(gameObject);
        }

        // El proyectil se destruye al impactar con elementos del escenario (paredes, obstáculos).
        if (collision.CompareTag("Environment") || collision.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}