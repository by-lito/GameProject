using UnityEngine;

/// <summary>
/// Attach this alongside any LobbyUnlockableItem (or any Item subclass) to make it
/// a physical pickup inside a run.
///
/// PREFAB STRUCTURE (one per item type):
///   ItemPickup_SpeedBoost         ← name it clearly
///     ├── ItemSpeedBoost.cs       ← the item effect
///     ├── RunItemPickup.cs        ← this script (handles 3D pickup)
///     ├── SphereCollider          ← Is Trigger = true, radius ~0.8
///     ├── SpriteRenderer          ← item icon floating in world
///     └── Billboard               ← faces camera
///
/// HOW IT WORKS:
///   - On Start: checks item.isUnlocked (from PlayerPrefs)
///     → if not unlocked: hides the GameObject (not available in run)
///     → if unlocked: waits for player to press Action nearby
///   - On pickup: calls PlayerInventory.AddItem(item) → triggers OnEquip(player)
///     → stats are applied immediately for the current run
///   - Then destroys itself
/// </summary>
[RequireComponent(typeof(Item))]
public class RunItemPickup : MonoBehaviour
{
    [Header("Interaction")]
    public float pickupRange = 1.5f;
    public GameObject promptUI; // "Pulsa E para recoger [itemName]" panel

    private Item item;
    private PlayerController playerController;
    private bool pickedUp = false;

    void Start()
    {
        item = GetComponent<Item>();

        if (item == null)
        {
            Debug.LogError("[RunItemPickup] No Item component on this GameObject.");
            return;
        }

        // Only available in run if player has unlocked it in the lobby
        if (!item.isUnlocked)
        {
            gameObject.SetActive(false);
            return;
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.OnActionPressed += TryPickup;
        }

        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (pickedUp || playerController == null) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);
        if (promptUI != null)
            promptUI.SetActive(dist <= pickupRange);
    }

    private void TryPickup()
    {
        if (pickedUp || playerController == null || item == null) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);
        if (dist > pickupRange) return;

        pickedUp = true;
        if (promptUI != null) promptUI.SetActive(false);

        // Add to inventory → triggers OnEquip → applies stat modifier
        PlayerInventory inv = playerController.GetComponent<PlayerInventory>();
        if (inv != null)
            inv.AddItem(item);
        else
            item.OnEquip(playerController.gameObject); // fallback

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (playerController != null)
            playerController.OnActionPressed -= TryPickup;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}