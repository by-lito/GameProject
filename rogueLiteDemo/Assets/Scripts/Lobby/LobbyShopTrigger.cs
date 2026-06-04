using UnityEngine;

/// <summary>
/// Attach to the shop prefab in the Lobby scene.
/// Opens the LobbyShopUI when the player presses Action while inside the trigger zone.
///
/// PREFAB STRUCTURE:
///   LobbyShopObject
///     ├── SphereCollider       (Is Trigger = true, radius ~2.5)
///     ├── LobbyShopTrigger     (this script)
///     ├── SpriteRenderer       (shop visual)
///     └── Billboard
///
/// SCENE SETUP:
///   - LobbyShopCanvas (separate Canvas in scene) with LobbyShopUI attached
///   - Assign it to shopUIPanel below
/// </summary>
public class LobbyShopTrigger : MonoBehaviour
{
    [Header("UI")]
    public LobbyShopUI shopUI;          // Drag the LobbyShopUI panel here
    public GameObject interactPrompt;   // "Pulsa E para abrir la tienda" label

    private PlayerController playerController;
    private bool playerInRange = false;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (shopUI != null) shopUI.Close();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;

        playerInRange = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);

        playerController.OnActionPressed += TryOpen;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (playerController != null)
        {
            playerController.OnActionPressed -= TryOpen;
            playerController = null;
        }

        if (shopUI != null) shopUI.Close();
    }

    private void TryOpen()
    {
        if (!playerInRange || playerController == null) return;

        if (shopUI != null && !shopUI.IsOpen)
        {
            shopUI.Open(playerController);
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (playerController != null)
            playerController.OnActionPressed -= TryOpen;
    }
}