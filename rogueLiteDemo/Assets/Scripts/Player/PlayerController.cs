using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float moveSpeed = 6f;
    private Rigidbody rb;
    private Vector2 moveInput;

    [Header("Ajustes de Combate")]
    public Transform attackPoint;
    public float attackRange = 0.8f;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que el personaje se caiga de lado
        rb.linearDamping = 5f;
    }

    // Input System: Movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

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

    // Input System: Ataque Melee
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(
                attackPoint.position,
                attackRange,
                enemyLayers
            );

            foreach (Collider enemy in hitEnemies)
            {
                print("¡Enemigo golpeado: " + enemy.name + "!");
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(10f);
                }
            }
        }
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(ExecuteDash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        // En el mapa 3D, el movimiento es en X y Z
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = move * moveSpeed;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}