using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss Phase 1.
/// Inherits from EnemyBase. Overrides movement (flees player) and death (triggers Phase 2).
///
/// ATTACKS:
///   A) Summon   — spawns 5 EnemyInmobilizer sequentially
///   B) Projectile — rapid-fire machine-gun burst
///   C) HealZone — places healing zones on ground that restore boss HP
///
/// RULES:
///   - Executes 1 or 2 attacks at a time (never all 3)
///   - Dual-attack adds a penalty cooldown
///   - Does NOT move while attacking
///   - Flees player when not attacking
///   - On death: disappears and spawns Phase 2 prefab instead of dying normally
///
/// SETUP (Inspector):
///   BossPhase1Root
///     ├── Health          (maxHP = 500+)
///     ├── Rigidbody       (isKinematic = true recommended)
///     ├── Collider        (not trigger — for physics)
///     ├── BossPhase1      (this script)
///     └── SpriteChild
///           ├── SpriteRenderer
///           └── Billboard
///
///   Assign in Inspector:
///     - inmobilizerPrefab  → EnemyInmobilizer prefab
///     - projectilePrefab   → HealerProjectile prefab (reused)
///     - healingZonePrefab  → HealingZone prefab
///     - shootPoint         → child Transform
///     - phase2Prefab       → BossPhase2 prefab
/// </summary>
public class BossPhase1 : EnemyBase
{
    // ── Inspector ────────────────────────────────────────────────────

    [Header("Boss Stats")]
    public float fleeSpeed = 2.5f;
    public float minFleeDistance = 6f;       // Boss tries to stay at least this far

    [Header("Attack Timing")]
    public float baseAttackCooldown = 4f;
    public float dualAttackPenalty = 3f;     // Extra cooldown when 2 attacks fire together
    [Range(0f, 1f)]
    public float dualAttackChance = 0.35f;   // Probability of triggering 2 attacks at once

    [Header("A) Summon Attack")]
    public GameObject inmobilizerPrefab;
    public int summonCount = 5;
    public float summonDelay = 0.4f;         // Delay between each spawn
    public float summonRadius = 3f;

    [Header("B) Projectile Attack")]
    public GameObject projectilePrefab;      // Reuse HealerProjectile
    public Transform shootPoint;
    public int projectileBurst = 8;          // Number of projectiles per burst
    public float projectileInterval = 0.12f; // Time between each shot
    public float projectileHealAmount = 8f;
    public float projectileKnockback = 5f;

    [Header("C) Healing Zone Attack")]
    public GameObject healingZonePrefab;
    public int zoneCount = 2;
    public float zoneRadius = 4f;            // Spawn radius around boss
    public float zoneHealPerSecond = 6f;
    public float zoneDuration = 5f;

    [Header("Phase 2 Transition")]
    public GameObject phase2Prefab;
    public float deathFadeDuration = 1f;

    // ── State ─────────────────────────────────────────────────────────

    private enum AttackType { Summon, Projectile, HealZone }

    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float currentCooldown;

    // ── Init ──────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
        currentCooldown = baseAttackCooldown;
        angelDustValue = 0; // Phase 1 death is handled by phase transition, not dust
    }

    // ── Main Loop ─────────────────────────────────────────────────────

    protected override void Update()
    {
        if (player == null) return;

        if (isAttacking) return; // No movement, no new attacks while busy

        attackTimer += Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        // Only act if player is in detection range
        if (dist > detectionRange) return;

        if (attackTimer >= currentCooldown)
        {
            attackTimer = 0f;
            StartCoroutine(ExecuteAttackRound());
        }
        else
        {
            FleeFromPlayer(dist);
        }
    }

    // ── Movement: Flee ────────────────────────────────────────────────

    private void FleeFromPlayer(float currentDist)
    {
        if (currentDist >= minFleeDistance) return; // Already far enough

        Vector3 fleeDir = (transform.position - player.position).normalized;
        transform.position += fleeDir * fleeSpeed * Time.deltaTime;
    }

    // Override base — boss never chases
    protected override void MoveToPlayer() { }
    protected override void Attack() { }

    // ── Attack Round ──────────────────────────────────────────────────

    private IEnumerator ExecuteAttackRound()
    {
        isAttacking = true;

        bool isDual = Random.value < dualAttackChance;

        if (isDual)
        {
            // Pick 2 different attacks
            List<AttackType> pool = new List<AttackType> {
                AttackType.Summon,
                AttackType.Projectile,
                AttackType.HealZone
            };

            int indexA = Random.Range(0, pool.Count);
            AttackType attackA = pool[indexA];
            pool.RemoveAt(indexA);
            AttackType attackB = pool[Random.Range(0, pool.Count)];

            // Launch both simultaneously
            Coroutine a = StartCoroutine(RunAttack(attackA));
            Coroutine b = StartCoroutine(RunAttack(attackB));

            // Wait for both to finish
            yield return a;
            yield return b;

            // Dual-attack penalty
            currentCooldown = baseAttackCooldown + dualAttackPenalty;
        }
        else
        {
            // Single random attack
            AttackType[] all = { AttackType.Summon, AttackType.Projectile, AttackType.HealZone };
            AttackType chosen = all[Random.Range(0, all.Length)];

            yield return StartCoroutine(RunAttack(chosen));

            currentCooldown = baseAttackCooldown;
        }

        isAttacking = false;
    }

    private IEnumerator RunAttack(AttackType type)
    {
        switch (type)
        {
            case AttackType.Summon:
                yield return StartCoroutine(SummonAttack());
                break;
            case AttackType.Projectile:
                yield return StartCoroutine(ProjectileAttack());
                break;
            case AttackType.HealZone:
                yield return StartCoroutine(HealZoneAttack());
                break;
        }
    }

    // ── A) Summon Attack ──────────────────────────────────────────────

    private IEnumerator SummonAttack()
    {
        if (inmobilizerPrefab == null)
        {
            Debug.LogWarning("[BossPhase1] inmobilizerPrefab not assigned.");
            yield break;
        }

        for (int i = 0; i < summonCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * summonRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, 0f, offset.y);
            Instantiate(inmobilizerPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(summonDelay);
        }
    }

    // ── B) Projectile Attack ──────────────────────────────────────────

    private IEnumerator ProjectileAttack()
    {
        if (projectilePrefab == null || shootPoint == null)
        {
            Debug.LogWarning("[BossPhase1] projectilePrefab or shootPoint not assigned.");
            yield break;
        }

        for (int i = 0; i < projectileBurst; i++)
        {
            FireProjectile();
            yield return new WaitForSeconds(projectileInterval);
        }
    }

    private void FireProjectile()
    {
        if (player == null) return;

        Vector3 direction = (player.position - shootPoint.position).normalized;

        // Use Quaternion.identity so the Sprite doesn't rotate out of view
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        HealerProjectile hp = proj.GetComponent<HealerProjectile>();
        if (hp != null)
        {
            hp.SetDirection(direction); // <--- Pass the direction here
            hp.healAmount = projectileHealAmount;
            hp.knockbackForce = projectileKnockback;
        }
    }

    // ── C) Healing Zone Attack ────────────────────────────────────────

    private IEnumerator HealZoneAttack()
    {
        if (healingZonePrefab == null)
        {
            Debug.LogWarning("[BossPhase1] healingZonePrefab not assigned.");
            yield break;
        }

        for (int i = 0; i < zoneCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * zoneRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, 0f, offset.y);

            GameObject zoneObj = Instantiate(healingZonePrefab, spawnPos, Quaternion.identity);

            HealingZone zone = zoneObj.GetComponent<HealingZone>();
            if (zone != null)
            {
                zone.bossHealth = health;
                zone.healPerSecond = zoneHealPerSecond;
                zone.duration = zoneDuration;
                zone.Activate();
            }
        }

        // HealZone attack completes immediately after spawning — zones run independently
        yield return null;
    }

    // ── Death: Phase Transition ───────────────────────────────────────

    protected override void HandleDeath()
    {
        // Do NOT call base — no dust drop, no standard destroy
        StopAllCoroutines();
        StartCoroutine(PhaseTransition());
    }

    private IEnumerator PhaseTransition()
    {
        // Disable all behaviour
        isAttacking = true; 

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            float elapsed = 0f;
            Color startColor = sr.color;

            while (elapsed < deathFadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / deathFadeDuration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(deathFadeDuration);
        }

        // Spawn Phase 2
        if (phase2Prefab != null)
            Instantiate(phase2Prefab, transform.position, Quaternion.identity);
        else
            Debug.LogWarning("[BossPhase1] phase2Prefab not assigned — Phase 2 won't spawn.");

        Destroy(gameObject);
    }

    // ── Gizmos ────────────────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Flee distance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minFleeDistance);

        // Summon radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, summonRadius);

        // Heal zone radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}