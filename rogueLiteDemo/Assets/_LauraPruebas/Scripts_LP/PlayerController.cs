using UnityEngine;
using UnityEngine.InputSystem; // Imprescindible para usar el nuevo sistema de Input de Unity 

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Ajustes de Combate")]
    public Transform attackPoint;    // El objeto vacío que pusimos delante del Player
    public float attackRange = 0.8f; // El radio del círculo rojo (Gizmo)
    public LayerMask enemyLayers;    // Aquí seleccionamos "Enemies" en el Inspector

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Unity 6: Ajustes para que el movimiento sea fluido y no flote
        rb.freezeRotation = true;
        rb.linearDamping = 5f;
    }

    // Se dispara con el Player Input (Action: Move)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Se dispara con el Player Input (Action: Attack)
    public void OnAttack(InputValue value)
    {
        // Solo atacamos cuando se pulsa el botón (isPressed)
        if (value.isPressed)
        {
            Debug.Log("Laura: El Player ha lanzado un ataque");

            // Crear un círculo invisible y detectar qué colisionadores de la capa "Enemies" toca
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                // Buscamos el script de vida en el enemigo que hemos golpeado
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(10f); // El daño que acordamos ayer
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Aplicar movimiento físico
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // Dibujamos el círculo en el Editor para que veamos el alcance del ataque
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}