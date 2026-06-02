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

    // [NUEVO] Referencia al componente Animator del sprite Player_Walk_012 hijo
    private Animator anim;

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
    /// Evento que se dispara al pulsar el bot�n de Acci�n.
    /// Utilizado para interacciones con el entorno y fases del Jefe.
    /// </summary>
    public System.Action OnActionPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 5f;

        // [NUEVO] Busca el Animator en sus componentes hijos autom�ticamente
        anim = GetComponentInChildren<Animator>();
    }

    // --- M�TODOS DE ENTRADA (INPUT SYSTEM) ---

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
    /// Ejecuta un ataque de �rea circular (Melee) frente al jugador.
    /// </summary>
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || isParalyzed || isDashing) return;

        // Activamos el trigger de ataque para reproducir la animaci�n
        if (anim != null) anim.SetTrigger("isAttacking"); 
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
    /// Instancia un proyectil en la posici�n de disparo.
    /// </summary>
    public void OnFire(InputValue value)
    {
        if (value.isPressed && !isParalyzed && !isDashing)
        {
            // Activamos el trigger de disparo para reproducir la animaci�n
            if (anim != null) anim.SetTrigger("isShooting");

            if (projectilePrefab != null && shootPoint != null)
            {
                // 1. Instanciamos el proyectil verde en el ShootPoint
                GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

                // 2. Obtenemos la �ltima direcci�n en la que miraba Aurora desde los par�metros de su Animator
                Vector2 fireDirection = new Vector2(anim.GetFloat("Horizontal"), anim.GetFloat("Vertical"));

                // Si por lo que sea da 0 (ej. al empezar), disparamos hacia abajo o al frente por defecto
                if (fireDirection.sqrMagnitude < 0.01f) fireDirection = Vector2.down;

                // 3. Le pasamos los datos al script del proyectil
                Projectile scriptProyectil = bullet.GetComponent<Projectile>();
                if (scriptProyectil != null)
                {
                    scriptProyectil.SetupDirection(fireDirection);
                }
            }
        }
    }

    /// <summary>
    /// Activa la habilidad de Dash si est� disponible y el jugador no est� paralizado.
    /// </summary>
    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing && !isParalyzed)
        {
            StartCoroutine(ExecuteDash());
        }
    }

    /// <summary>
    /// Invoca el evento de acci�n para interactuar con objetos o jefes.
    /// </summary>
    public void OnAction(InputValue value)
    {
        if (!value.isPressed) return;
        OnActionPressed?.Invoke();
    }

    // --- L�GICA DE HABILIDADES ---

    /// <summary>
    /// Corrutina que gestiona el desplazamiento r�pido (Dash) y su tiempo de reutilizaci�n.
    /// </summary>
    private IEnumerator ExecuteDash()
    {
        canDash = false;
        isDashing = true;

        if (anim != null) anim.SetBool("isDashing", true);// [NUEVO] Activamos el par�metro isDashing en el Animator para cambiar a la animaci�n de dash

        float originalDrag = rb.linearDamping;
        rb.linearDamping = 0f;

        Vector3 dashDir = moveInput == Vector2.zero
            ? transform.forward
            : new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearDamping = originalDrag;
        isDashing = false;
        if (anim != null) anim.SetBool("isDashing", false);// [NUEVO] Desactivamos el par�metro isDashing para volver a la animaci�n normal

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    /// <summary>
    /// Cambia el estado de par�lisis del jugador.
    /// </summary>
    /// <param name="state">True para paralizar, False para liberar.</param>
    public void SetParalyzed(bool state)
    {
        isParalyzed = state;
        if (state)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            // [NUEVO] Si est� paralizado, paramos la animaci�n de golpe
            if (anim != null) anim.SetFloat("Speed", 0f);
        }
    }

    void Update()
    {
        // Actualizamos el Animator en cada frame con los datos de las teclas o del estado
        if (anim != null && !isParalyzed)
        {
            // CASO 1: Si nos estamos moviendo de forma normal caminando (NO estamos haciendo dash)
            if (moveInput.sqrMagnitude > 0.01f && !isDashing)
            {
                //Pasamos los controles limpios en directo (sin el * -1f) para que responda bien a la lista original de Unity
                anim.SetFloat("Horizontal", moveInput.x);
                anim.SetFloat("Vertical", moveInput.y);
            }
            // CASO 2: Si estamos haciendo un Dash 
            else if (isDashing)
            {
                // Si est�s pulsando alguna tecla de direcci�n durante el Dash, usamos esa direcci�n limpia
                if (moveInput.sqrMagnitude > 0.01f)
                {
                    anim.SetFloat("Horizontal", moveInput.x);
                    anim.SetFloat("Vertical", moveInput.y);
                }
                else
                {
                    // Si haces un dash est�tico, leemos el Rigidbody mapeando el eje Z del mundo 3D a la Y del Animator
                    Vector3 dashDirection = rb.linearVelocity.normalized;
                    if (dashDirection.sqrMagnitude > 0.01f)
                    {
                        anim.SetFloat("Horizontal", dashDirection.x);
                        anim.SetFloat("Vertical", dashDirection.z);
                    }
                }
            }

            // Pasamos la velocidad (magnitud) al par�metro Speed para cambiar entre Idle y Walk
            float currentSpeed = isDashing ? 1f : moveInput.magnitude;
            anim.SetFloat("Speed", currentSpeed);
        }
    }

    void FixedUpdate()
    {
        if (isDashing || isParalyzed) return;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = move * moveSpeed;
    }

    /// <summary>
    /// Dibuja el rango de ataque en el Editor de Unity para facilitar el ajuste de par�metros.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}