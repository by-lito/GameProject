using System.Collections;
using UnityEngine;

/// <summary>
/// Spawned by BossPhase1. Appears, heals the boss over time, then disappears.
/// Assign a visual (cylinder/disc) as child — scale it in the Inspector.
/// </summary>
public class HealingZone : MonoBehaviour
{
    [HideInInspector] public Health bossHealth;
    [HideInInspector] public float healPerSecond = 5f;
    [HideInInspector] public float duration = 5f;

    public void Activate()
    {
        StartCoroutine(ZoneLifecycle());
    }

    private IEnumerator ZoneLifecycle()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (bossHealth != null)
                bossHealth.Heal(healPerSecond * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}