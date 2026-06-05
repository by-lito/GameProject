using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El EnemySpawner de esta sala.")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Tooltip("Portal(es) de salida. Se apagan al empezar y se encienden al limpiar la sala.")]
    [SerializeField] private List<GameObject> exitPortals = new List<GameObject>();

    [Tooltip("Recompensa de fin de sala (opcional). Se suelta al limpiar la sala.")]
    [SerializeField] private RoomReward roomReward;

    private bool roomCleared = false;

    private void Awake()
    {
        SetPortalsActive(false);
    }

    private void OnEnable()
    {
        if (enemySpawner != null)
            enemySpawner.OnAllEnemiesDefeated += HandleSpawnerCleared;
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
            enemySpawner.OnAllEnemiesDefeated -= HandleSpawnerCleared;
    }

    private void Start()
    {
        if (enemySpawner == null)
            Debug.LogError("[RoomController] No se ha asignado 'Enemy Spawner'.", this);

        if (exitPortals == null || exitPortals.Count == 0)
            Debug.LogWarning("[RoomController] No hay portales en 'Exit Portals'.", this);
    }

    private void HandleSpawnerCleared()
    {
        if (roomCleared) return;
        StartCoroutine(WaitUntilNoEnemiesLeft());
    }

    private IEnumerator WaitUntilNoEnemiesLeft()
    {
        yield return null;

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            yield return new WaitForSeconds(0.25f);

        ClearRoom();
    }

    private void ClearRoom()
    {
        if (roomCleared) return;
        roomCleared = true;

        if (roomReward != null) roomReward.SpawnRewards();

        SetPortalsActive(true);
        Debug.Log("[RoomController] Sala limpia. Recompensa soltada y portal(es) activado(s).", this);
    }

    private void SetPortalsActive(bool active)
    {
        foreach (GameObject portal in exitPortals)
            if (portal != null) portal.SetActive(active);
    }
}