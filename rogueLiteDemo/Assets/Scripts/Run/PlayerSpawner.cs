using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] 'Player Prefab' no asignado.", this);
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[PlayerSpawner] 'Spawn Point' no asignado.", this);
            return;
        }

        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Debug.LogWarning("[PlayerSpawner] Ya existe un Player. No se instancia otro.", this);
            return;
        }

        GameObject player = Instantiate(playerPrefab);
        player.name = playerPrefab.name;

        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        Debug.Log("[PlayerSpawner] Player instanciado en " + spawnPoint.position, this);
    }
}