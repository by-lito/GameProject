using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawned by BossPhase2 on hug interaction.
/// Player picks it up with the Action button when close enough.
/// On pickup: adds to inventory, ends run, returns to lobby.
/// Requires: Collider (Is Trigger = true), tag "Item" optional.
/// </summary>
public class PotionFragment : MonoBehaviour
{
    [Header("Interaction")]
    public float pickupRange = 1.5f;
    public string lobbyScene = "Lobby_3D";

    [Header("UI")]
    public GameObject promptUI; // "Press [E] to pick up" panel

    private PlayerController playerController;
    private bool pickedUp = false;

    void Start()
    {
        // Find player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.OnActionPressed += TryPickup;
        }

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (pickedUp || playerController == null) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);
        bool inRange = dist <= pickupRange;

        if (promptUI != null)
            promptUI.SetActive(inRange);
    }

    private void TryPickup()
    {
        if (pickedUp || playerController == null) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);
        if (dist > pickupRange) return;

        pickedUp = true;
        Pickup();
    }

    private void Pickup()
    {
        // Add to inventory if available
        PlayerInventory inventory = playerController.GetComponent<PlayerInventory>();
        if (inventory != null)
            Debug.Log("[PotionFragment] Added to inventory."); // Replace with inventory.AddItem(...) once item SO exists

        // Notify GameManager run ended successfully
        GameManager.Instance?.EndRun(playerDied: false);

        // Return to lobby
        SceneManager.LoadScene(lobbyScene);
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (playerController != null)
            playerController.OnActionPressed -= TryPickup;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}