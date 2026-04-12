using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;

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
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

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

    void FixedUpdate()
    {
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