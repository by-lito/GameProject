using UnityEngine;

/// <summary>
/// Run shop vendor. Place on each item vendor object in Room_Shop_01.
/// Uses OnActionPressed (Input System) — NOT Input.GetKeyDown.
/// Spends AngelDust (run currency from PlayerWallet).
///
/// PREFAB STRUCTURE per vendor slot:
///   ShopVendor
///     ├── SphereCollider     Is Trigger = true, radius ~2
///     ├── ShopSystem         (this script)
///     ├── SpriteRenderer     vendor visual
///     └── Billboard
///
/// Inspector setup:
///   shopMode  = Run
///   itemToSell = drag the Item component from a child GO
///   price     = cost in AngelDust
/// </summary>
public class ShopSystem : MonoBehaviour
{
    public enum ShopType { Run }   // Lobby mode removed — use LobbyShopUI instead

    [Header("Configuración")]
    public ShopType shopMode = ShopType.Run;
    public Item itemToSell;
    public int price;

    [Header("UI")]
    public GameObject interactPrompt;   // "Pulsa E para comprar" panel

    private PlayerController playerController;
    private bool playerInRange = false;
    private bool sold = false;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (sold || !other.CompareTag("Player")) return;

        playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;

        playerInRange = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);
        playerController.OnActionPressed += TryPurchase;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (playerController != null)
        {
            playerController.OnActionPressed -= TryPurchase;
            playerController = null;
        }
    }

    private void TryPurchase()
    {
        if (sold || !playerInRange || playerController == null) return;
        if (itemToSell == null)
        {
            Debug.LogWarning("[ShopSystem] itemToSell not assigned.");
            return;
        }

        if (!PlayerWallet.instance.CanAfford(price))
        {
            Debug.Log($"[Shop] AngelDust insuficiente. Necesitas {price}, tienes {PlayerWallet.instance.angelDust}.");
            return;
        }

        PlayerWallet.instance.SpendDust(price);
        PlayerInventory.instance.AddItem(itemToSell);
        Debug.Log($"[Shop] Comprado: {itemToSell.itemName}");

        sold = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (playerController != null) playerController.OnActionPressed -= TryPurchase;

        // Optional: hide vendor visually
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }

    void OnDestroy()
    {
        if (playerController != null)
            playerController.OnActionPressed -= TryPurchase;
    }
}