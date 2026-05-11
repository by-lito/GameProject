using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Clase principal para el control del jugador. 
/// Gestiona movimiento en 3D, combate (melee y rango), dash y estados alterados.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [Tooltip("Velocidad de movimiento base del jugador.")]
    public float moveSpeed = 6f;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isParalyzed = false;

    [Header("Ajustes de Combate")]
    public Transform attackPoint;
    public float attackRange = 0.8f;
    public float meleeDamage = 10f;
    public LayerMask enemyLayers;

    [Header("Ajustes de Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    [Header("Ataque a Rango")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    /// <summary>
    /// Evento que se dispara al pulsar el botón de Acción.
    /// Utilizado para interacciones con el entorno y fases del Jefe.
    /// </summary>
    public System.Action OnActionPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 5f;
    }

    // --- MÉTODOS DE ENTRADA (INPUT SYSTEM) ---

    /// <summary>
    /// Recibe la entrada de movimiento del Input System.
    /// </summary>
    /// <param name="value">Valor vectorial (X, Y) de la entrada.</param>
    public void OnMove(InputValue value)
    {
        if (isParalyzed || isDashing) return;
        moveInput = value.Get<Vector2>();
    }

    /// <summary>
    /// Ejecuta un ataque de área circular (Melee) frente al jugador.
    /// </summary>
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || isParalyzed || isDashing) return;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable dmg = enemy.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(meleeDamage);
            else
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null) enemyHealth.TakeDamage(meleeDamage);
            }
        }
    }

    /// <summary>
    /// Instancia un proyectil en la posición de disparo.
    /// </summary>
    public void OnFire(InputValue value)
    {
        if (value.isPressed && !isParalyzed && !isDashing)
        {
            if (projectilePrefab != null && shootPoint != null)
            {
                Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            }
        }
    }

    /// <summary>
    /// Activa la habilidad de Dash si está disponible y el jugador no está paralizado.
    /// </summary>
    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing && !isParalyzed)
        {
            StartCoroutine(ExecuteDash());
        }
    }

    /// <summary>
    /// Invoca el evento de acción para interactuar con objetos o jefes.
    /// </summary>
    public void OnAction(InputValue value)
    {
        if (!value.isPressed) return;
        OnActionPressed?.Invoke();
    }

    // --- LÓGICA DE HABILIDADES ---

    /// <summary>
    /// Corrutina que gestiona el desplazamiento rápido (Dash) y su tiempo de reutilización.
    /// </summary>
    private IEnumerator ExecuteDash()
    {
        canDash = false;
        isDashing = true;

        float originalDrag = rb.linearDamping;
        rb.linearDamping = 0f;

        Vector3 dashDir = moveInput == Vector2.zero
            ? transform.forward
            : new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearDamping = originalDrag;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    /// <summary>
    /// Cambia el estado de parálisis del jugador.
    /// </summary>
    /// <param name="state">True para paralizar, False para liberar.</param>
    public void SetParalyzed(bool state)
    {
        isParalyzed = state;
        if (state)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void FixedUpdate()
    {
        if (isDashing || isParalyzed) return;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        // Mantenemos la velocidad vertical (gravedad) mientras aplicamos velocidad en X y Z
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }

    /// <summary>
    /// Dibuja el rango de ataque en el Editor de Unity para facilitar el ajuste de parámetros.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}