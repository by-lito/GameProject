using System.Collections;
using UnityEngine;

/// <summary>
/// Static high-health enemy that spawns EnemyInmobilizer every 6 seconds.
/// Does not move. Drops AngelDust on death.
/// </summary>
public class InmobilizerNest : EnemyBase
{
    [Header("Spawning")]
    public GameObject inmobilizerPrefab;
    public float spawnInterval = 6f;
    public int maxActiveInmobilizers = 4;
    public float spawnRadius = 1.5f;

    private int activeCount = 0;

    protected override void Awake()
    {
        base.Awake();
        angelDustValue = 25;
        moveSpeed = 0f; // Static
    }

    protected override void Update()
    {
        // Static: no movement, no base AI loop
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeCount < maxActiveInmobilizers)
                SpawnInmobilizer();
        }
    }

    private void SpawnInmobilizer()
    {
        if (inmobilizerPrefab == null)
        {
            Debug.LogWarning("InmobilizerNest: inmobilizerPrefab not assigned.");
            return;
        }

        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);

        GameObject spawned = Instantiate(inmobilizerPrefab, spawnPos, Quaternion.identity);
        activeCount++;

        // Track death to update active count
        Health spawnedHealth = spawned.GetComponent<Health>();
        if (spawnedHealth != null)
            spawnedHealth.OnDeath += () => activeCount--;
    }

    // Override to prevent base Attack
    protected override void Attack() { }
    protected override void MoveToPlayer() { }
}