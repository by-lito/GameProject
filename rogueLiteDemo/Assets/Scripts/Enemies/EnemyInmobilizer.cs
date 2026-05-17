using System.Collections;
using UnityEngine;

/// <summary>
/// Chases the player constantly.
/// On collision: applies paralyze (2s) then dies immediately.
/// Dies in 1 hit. Does NOT drop AngelDust.
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

        // Always chase, no idle range check
        MoveToPlayer();
    }

    protected override void Attack() { }

    private void OnTriggerEnter(Collider other)
    {
        if (hasParalyzed) return;
        if (!other.CompareTag("Player")) return;

        hasParalyzed = true;
        StartCoroutine(ParalyzeAndDie(other.gameObject));
    }

    private IEnumerator ParalyzeAndDie(GameObject playerObj)
    {
        // Apply paralyze via PlayerController if it exposes the interface
        PlayerController pc = playerObj.GetComponent<PlayerController>();
        if (pc != null)
            pc.SetParalyzed(true);

        yield return new WaitForSeconds(paralyzeDuration);

        if (pc != null)
            pc.SetParalyzed(false);

        // Die without dropping anything
        Destroy(gameObject);
    }

    protected override void HandleDeath()
    {
        // 1-hit kill: no dust, just destroy
        Destroy(gameObject);
    }
}