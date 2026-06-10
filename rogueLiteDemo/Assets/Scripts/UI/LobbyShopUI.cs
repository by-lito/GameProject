using TMPro;
using UnityEngine;

/// <summary>
/// Main Lobby Shop UI manager. Attach to the ShopPanel GameObject inside the Canvas.
///
/// CANVAS STRUCTURE:
///   LobbyShopCanvas  (Canvas - Screen Space Overlay)
///   └── ShopPanel    (this script + Image background)
///       ├── TitleText        (TMP_Text)   "Tienda del Lobby"
///       ├── CoinsDisplay     (TMP_Text)   "Monedas: 50"
///       ├── SlotsContainer   (Vertical Layout Group)
///       │     ├── Slot0  (LobbyItemSlot prefab)
///       │     ├── Slot1
///       │     ├── Slot2
///       │     └── Slot3
///       └── CloseButton      (Button → calls Close())
///
/// SETUP:
///   1. Build the canvas structure above.
///   2. Assign the 4 LobbyUnlockableItem components (from LobbyItemsHolder) to lobbyItems[].
///   3. Assign the 4 LobbyItemSlot components to slots[].
///   4. Assign CoinsDisplay TMP_Text.
///   5. Wire CloseButton.onClick → Close().
///   6. Drag this ShopPanel GameObject into LobbyShopTrigger.shopUI.
/// </summary>
public class LobbyShopUI : MonoBehaviour
{
    [Header("Items — assign in Inspector (order matters)")]
    public LobbyUnlockableItem[] lobbyItems = new LobbyUnlockableItem[4];

    [Header("Slots — one per item (same order)")]
    public LobbyItemSlot[] slots = new LobbyItemSlot[4];

    [Header("UI")]
    public TMP_Text coinsDisplay;

    public bool IsOpen { get; private set; }

    private PlayerController currentPlayer;



    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(PlayerController player)
    {
        currentPlayer = player;
        IsOpen = true;
        gameObject.SetActive(true);

        RefreshAll();
    }

    public void Close()
    {
        IsOpen = false;
        gameObject.SetActive(false);
        currentPlayer = null;
    }

    private void RefreshAll()
    {
        if (currentPlayer == null) return;

        int coins = currentPlayer.coins;

        if (coinsDisplay != null)
            coinsDisplay.text = $"Monedas: {coins}";

        for (int i = 0; i < slots.Length && i < lobbyItems.Length; i++)
        {
            if (slots[i] != null && lobbyItems[i] != null)
            {
                slots[i].Setup(lobbyItems[i], this);
                slots[i].Refresh(coins);
            }
        }
    }

    /// <summary>Called by LobbyItemSlot when the buy button is pressed.</summary>
    public void OnSlotPurchase(LobbyUnlockableItem item)
    {
        if (currentPlayer == null || item == null) return;

        bool success = item.TryBuy(currentPlayer);

        if (success)
            RefreshAll(); // Update all slots — buying one changes coins for others
    }
}