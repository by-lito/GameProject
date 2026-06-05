using System.Collections;
using UnityEngine;

/// <summary>
/// Ranged enemy that heals the player on hit (inverted health mechanic).
/// Stays still while attacking, then relocates to a new position.
/// Applies knockback on hit. Drops AngelDust on death.
/// </summary>
public class EnemyHealer : EnemyBase
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float attackCooldown = 2f;

    [Header("Knockback")]
    public float knockbackForce = 8f;

    [Header("Relocation")]
    public float relocateRadius = 5f;
    public float immobileDuration = 1f;

    private float attackTimer;
    private bool isRelocating;
    private bool isImmobile;

    protected override void Awake()
    {
        base.Awake();
        angelDustValue = 15;
    }

    protected override void Update()
    {
        if (player == null || isRelocating) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > detectionRange) return;

        attackTimer += Time.deltaTime;

        if (dist <= attackRange && !isImmobile)
        {
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                StartCoroutine(AttackSequence());
            }
        }
        else if (!isImmobile)
        {
            MoveToPlayer();
        }
    }

    private IEnumerator AttackSequence()
    {
        // Immobilize while attacking
        isImmobile = true;

        FireProjectile();

        yield return new WaitForSeconds(immobileDuration);

        isImmobile = false;

        // Relocate after attacking
        StartCoroutine(Relocate());
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        Vector3 direction = (player.position - shootPoint.position).normalized;

        // Use Quaternion.identity so the Sprite doesn't rotate out of view
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        HealerProjectile hp = proj.GetComponent<HealerProjectile>();
        if (hp != null)
        {
            hp.SetDirection(direction); // <--- Pass the direction here
            hp.knockbackForce = knockbackForce;
            hp.healAmount = damage;
        }
    }

    private IEnumerator Relocate()
    {
        isRelocating = true;

        // Pick a random position around the player at relocateRadius distance
        Vector2 randomCircle = Random.insideUnitCircle.normalized * relocateRadius;
        Vector3 targetPos = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        float timeout = 3f;
        float elapsed = 0f;

        while (Vector3.Distance(transform.position, targetPos) > 0.3f && elapsed < timeout)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        isRelocating = false;
    }

    // Override Attack to prevent base class from calling it (handled by coroutine)
    protected override void Attack() { }
}