using System.Collections;
using UnityEngine;

/// <summary>
/// Chases the player constantly.
/// On collision with trigger zone: applies paralyze (2s) then dies.
/// Dies in 1 hit. Does NOT drop AngelDust.
///
/// PREFAB STRUCTURE (required for OnTriggerEnter to fire):
///   EnemyInmobilizerRoot
///     ├── CapsuleCollider      Is Trigger = FALSE  (physics, stops clipping through walls)
///     ├── EnemyInmobilizer     (this script)
///     ├── Health               maxHP = 1
///     └── TriggerZone          (child empty GO)
///           ├── SphereCollider Is Trigger = TRUE, Radius = 0.6
///           └── InmobilizerTrigger.cs  (calls OnPlayerContact on this script)
/// </summary>
public class EnemyInmobilizer : EnemyBase
{
    [Header("Paralyze")]
    public float paralyzeDuration = 2f;

    private bool hasParalyzed = false;

    protected override void Awake()
    {
        base.Awake();
        angelDustValue = 0; // No drop
    }

    protected override void Update()
    {
        if (player == null || hasParalyzed) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRange) return;

        MoveToPlayer(); // Always chase, no attack range threshold
    }

    protected override void Attack() { } // Handled by trigger collision, not range

    /// <summary>
    /// Called by InmobilizerTrigger.cs on the child TriggerZone.
    /// </summary>
    public void OnPlayerContact(GameObject playerObj)
    {
        if (hasParalyzed) return;
        hasParalyzed = true;
        StartCoroutine(ParalyzeAndDie(playerObj));
    }

    private IEnumerator ParalyzeAndDie(GameObject playerObj)
    {
        PlayerController pc = playerObj.GetComponent<PlayerController>();
        pc?.SetParalyzed(true);

        yield return new WaitForSeconds(paralyzeDuration);

        pc?.SetParalyzed(false);
        Destroy(gameObject);
    }

    protected override void HandleDeath()
    {
        // FIX: If killed externally while coroutine is running,
        // un-paralyze the player before destroying to prevent permanent paralysis.
        StopAllCoroutines();

        if (player != null)
            player.GetComponent<PlayerController>()?.SetParalyzed(false);

        Destroy(gameObject);
    }
}