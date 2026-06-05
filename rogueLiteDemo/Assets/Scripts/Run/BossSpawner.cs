using System.Collections;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss")]
    [Tooltip("Prefab de la Fase 1 del boss (Boss1).")]
    [SerializeField] private GameObject bossPrefab;

    [Tooltip("Punto donde aparece el boss.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Salida")]
    [Tooltip("Portal de salida. Se desactiva al empezar y se activa al derrotar al boss.")]
    [SerializeField] private GameObject exitPortal;

    [Tooltip("Cada cuánto comprueba si el boss ya no está (segundos).")]
    [SerializeField] private float checkInterval = 0.3f;

    private void Awake()
    {
        if (exitPortal != null) exitPortal.SetActive(false);
    }

    private void Start()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("[BossSpawner] No se ha asignado 'Boss Prefab'.", this);
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("[BossSpawner] No se ha asignado 'Spawn Point'.", this);
            return;
        }

        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("[BossSpawner] Boss instanciado en " + spawnPoint.position, this);

        StartCoroutine(WaitUntilBossDefeated());
    }

    private IEnumerator WaitUntilBossDefeated()
    {
        yield return null;

        while (FindAnyObjectByType<BossPhase1>() != null || FindAnyObjectByType<BossPhase2>() != null)
            yield return new WaitForSeconds(checkInterval);

        if (exitPortal != null) exitPortal.SetActive(true);
        Debug.Log("[BossSpawner] ¡Boss derrotado! Portal de salida activado.", this);
    }
}