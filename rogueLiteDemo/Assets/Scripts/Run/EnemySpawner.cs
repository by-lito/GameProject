using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [Header("Qué enemigos spawnear")]
    [Tooltip("Prefabs de enemigo. Para cada punto de spawn se elige uno al azar de esta lista.")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Dónde spawnearlos")]
    [Tooltip("Objeto vacío cuyos hijos son los puntos de spawn. Se crea un enemigo por cada hijo.")]
    [SerializeField] private Transform spawnPointsParent;

    public event Action OnAllEnemiesDefeated;

    public int AliveCount { get; private set; }

    private bool cleared = false;

    private void Start()
    {
        SpawnAll();
    }

    private void SpawnAll()
    {
        if (spawnPointsParent == null)
        {
            Debug.LogError("[EnemySpawner] No se ha asignado 'Spawn Points Parent'.", this);
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("[EnemySpawner] La lista 'Enemy Prefabs' está vacía.", this);
            return;
        }

        foreach (Transform point in spawnPointsParent)
        {
            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
            if (prefab == null) continue;

            GameObject enemy = Instantiate(prefab, point.position, point.rotation);

            Health health = enemy.GetComponent<Health>();
            if (health == null)
            {
                Debug.LogWarning("[EnemySpawner] '" + enemy.name + "' no tiene Health; no se contará su muerte.", enemy);
                continue;
            }

            AliveCount++;
            health.OnDeath += HandleEnemyDeath;
        }

        Debug.Log("[EnemySpawner] Enemigos spawneados: " + AliveCount, this);

        if (AliveCount == 0) Clear();
    }

    private void HandleEnemyDeath()
    {
        StatsTracker.Instance?.AddEnemyDefeated();
        AliveCount--;
        if (AliveCount <= 0) Clear();
    }

    private void Clear()
    {
        if (cleared) return;
        cleared = true;

        Debug.Log("[EnemySpawner] ¡Sala despejada! No quedan enemigos.", this);
        OnAllEnemiesDefeated?.Invoke();
    }
}