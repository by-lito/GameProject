using UnityEngine;

/// <summary>
/// Projectile fired by EnemyHealer.
/// Heals the player on hit (inverted health mechanic) and applies knockback.
/// </summary>
public class HealerProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float healAmount = 10f;
    public float knockbackForce = 8f;
    public float lifeTime = 4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Heal player (inverted mechanic: enemy projectile heals)
        IDamageable damageable = other.GetComponent<IDamageable>();
        damageable?.Heal(healAmount);

        // Apply knockback
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
            knockbackDir.y = 0f;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}