using System.Collections.Generic;
using UnityEngine;

public class RoomReward : MonoBehaviour
{
    [Header("Dónde aparecen")]
    [Tooltip("Punto donde aparecen. Si se deja vacío, aparecen en la posición de este objeto.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Dinero (siempre cae)")]
    [SerializeField] private GameObject coinPrefab;
    [Tooltip("Cuántas monedas caen al limpiar la sala.")]
    [SerializeField] private int coinCount = 3;
    [Tooltip("Separación entre monedas si cae más de una.")]
    [SerializeField] private float spreadRadius = 0.6f;

    [Header("Objeto (futuro, opcional)")]
    [Tooltip("Prefabs de objeto que PODRÍAN caer. Vacío por ahora.")]
    [SerializeField] private List<GameObject> possibleItems = new List<GameObject>();
    [Tooltip("Probabilidad (0 a 1) de que caiga un objeto al limpiar la sala.")]
    [Range(0f, 1f)]
    [SerializeField] private float itemDropChance = 0f;

    private Vector3 Origin => spawnPoint != null ? spawnPoint.position : transform.position;

    public void SpawnRewards()
    {
        SpawnCoins();
        TrySpawnItem();
    }

    private void SpawnCoins()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("[RoomReward] No hay 'Coin Prefab' asignado.", this);
            return;
        }

        for (int i = 0; i < coinCount; i++)
        {
            Vector2 offset = (coinCount > 1) ? Random.insideUnitCircle * spreadRadius : Vector2.zero;
            Vector3 pos = Origin + new Vector3(offset.x, 0f, offset.y);
            Instantiate(coinPrefab, pos, Quaternion.identity);
        }
    }

    private void TrySpawnItem()
    {
        if (possibleItems == null || possibleItems.Count == 0) return;
        if (itemDropChance <= 0f) return;
        if (Random.value > itemDropChance) return;

        GameObject item = possibleItems[Random.Range(0, possibleItems.Count)];
        if (item == null) return;

        Vector3 pos = Origin + new Vector3(0f, 0f, 1f);
        Instantiate(item, pos, Quaternion.identity);
    }
}