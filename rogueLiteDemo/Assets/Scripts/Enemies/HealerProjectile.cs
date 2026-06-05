using UnityEngine;

public class HealerProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float healAmount = 10f;
    public float knockbackForce = 8f;
    public float lifeTime = 4f;

    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 dir)
    {
        dir.y = 0f;
        moveDirection = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // FIX 1: Move Kinematic rigidbodies via FixedUpdate + MovePosition 
    // This allows the physics engine to calculate sweeps and register triggers!
    void FixedUpdate()
    {
        Vector3 nextPosition = transform.position + (moveDirection * speed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        // FIX 2: Check both the hit collider and its root parent for the "Player" tag
        if (!other.CompareTag("Player") && !other.transform.root.CompareTag("Player")) return;

        // FIX 3: Look for Health in parents in case it hits a child hitbox collider
        Health playerHealth = other.GetComponentInParent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(healAmount);
        }

        Rigidbody playerRb = other.GetComponentInParent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
            knockbackDir.y = 0f;
            playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}