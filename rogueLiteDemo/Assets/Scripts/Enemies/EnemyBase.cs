using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected Transform player;
    protected Health health;

    [Header("Stats")]
    public float moveSpeed = 3f;
    public float damage = 10f;

    [Header("AI")]
    public float detectionRange = 8f;
    public float attackRange = 2f;

    [Header("Economy")]
    public int angelDustValue = 0; // Set > 0 on enemies that should drop currency

    protected virtual void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
        else
            Debug.LogError("Player not found! Did you forget the Player tag?");

        health = GetComponent<Health>();

        if (health != null)
            health.OnDeath += HandleDeath;
        else
            Debug.LogError("Health component missing on enemy!");
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > detectionRange)
            return; // idle

        if (dist > attackRange)
            MoveToPlayer();
        else
            Attack();
    }

    protected virtual void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    protected virtual void Attack()
    {
        IDamageable dmg = player.GetComponent<IDamageable>();
        dmg?.TakeDamage(damage * Time.deltaTime);
    }

    protected virtual void HandleDeath()
    {
        DropAngelDust();
        Destroy(gameObject);
    }

    protected void DropAngelDust()
    {
        if (angelDustValue <= 0) return;

        PlayerWallet wallet = FindObjectOfType<PlayerWallet>();
        if (wallet != null)
            wallet.AddAngelDust(angelDustValue);
        else
            Debug.LogWarning("PlayerWallet not found in scene.");
    }
}