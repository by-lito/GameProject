using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;

    // ?? Paralysis (used by EnemyInmobilizer and BossPhase2) ??????????
    private bool isParalyzed = false;

    public void SetParalyzed(bool state)
    {
        isParalyzed = state;
        if (state) moveInput = Vector2.zero;
    }

    // ?? Action callback (used by BossPhase2 interaction) ?????????????
    // Subscribers register here to receive the "Action" button press
    public System.Action OnActionPressed;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value)
    {
        if (isParalyzed) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || isParalyzed) return;

        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider enemy in hitEnemies)
        {
            IDamageable dmg = enemy.GetComponent<IDamageable>();
            dmg?.TakeDamage(10f);
        }
    }

    /// <summary>
    /// Called by Unity Input System when the Action button is pressed.
    /// Map a button (e.g. E / South gamepad) to "Action" in your InputActions asset.
    /// </summary>
    public void OnAction(InputValue value)
    {
        if (!value.isPressed) return;
        OnActionPressed?.Invoke();
    }

    void FixedUpdate()
    {
        if (isParalyzed)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x * moveSpeed;
        velocity.z = move.z * moveSpeed;
        rb.linearVelocity = velocity;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}