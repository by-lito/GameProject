using System.Collections;
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

    [Header("Ajustes de Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    [Header("Ataque a Rango")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
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

    //Lógica del Dash
    private IEnumerator ExecuteDash()
    {
        canDash = false;
        isDashing = true;

        // Guardamos la gravedad para que no se caiga mientras dashea
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Si no te mueves, dash hacia donde miras. Si te mueves, hacia donde pulsas.
        Vector2 dashDir = moveInput == Vector2.zero ? (Vector2)transform.right : moveInput.normalized;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // Al terminar, devolvemos la gravedad y paramos el dash
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Esperamos el tiempo de recarga
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
    // Configura en el Input Action una acción llamada "Fire" (ej. Click derecho o Botón X)
    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
    }

    //Método para el Dash, se dispara con el Player Input (Action: Dash), tecla "Q".
    public void OnDash(InputValue value)
    {
        // Solo dasheamos si podemos y no estamos ya en medio de uno
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(ExecuteDash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return; //Si se dashea, no aplicamos el movimiento normal
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