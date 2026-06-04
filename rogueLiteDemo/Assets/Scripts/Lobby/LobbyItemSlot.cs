using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents one item row in the Lobby Shop UI.
/// Assign the child UI elements in the Inspector after building the prefab.
///
/// SLOT PREFAB STRUCTURE:
///   ItemSlot (Horizontal Layout Group)
///     ├── ItemIcon        (Image)
///     ├── InfoGroup
///     │     ├── ItemName  (TMP_Text)
///     │     └── ItemDesc  (TMP_Text)
///     ├── CostLabel       (TMP_Text)  e.g. "30 monedas"
///     └── BuyButton       (Button)
///           └── BuyLabel  (TMP_Text) "Comprar" / "Desbloqueado"
/// </summary>
public class LobbyItemSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;
    public TMP_Text costText;
    public Button buyButton;
    public TMP_Text buyButtonLabel;

    [Header("Colors")]
    public Color unlockedColor = new Color(0.4f, 0.9f, 0.4f);   // Green when unlocked
    public Color lockedColor = new Color(1f, 1f, 1f);           // White when available to buy
    public Color noFundsColor = new Color(0.6f, 0.6f, 0.6f);    // Grey when can't afford

    private LobbyUnlockableItem item;
    private LobbyShopUI shopUI;

    public void Setup(LobbyUnlockableItem lobbyItem, LobbyShopUI owner)
    {
        item = lobbyItem;
        shopUI = owner;

        if (itemIcon != null && item.icon != null) itemIcon.sprite = item.icon;
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemDescText != null) itemDescText.text = item.description;

        buyButton?.onClick.RemoveAllListeners();
        buyButton?.onClick.AddListener(OnBuyClicked);

        Refresh(0); // coins unknown at setup, shopUI will call Refresh after open
    }

    /// <summary>Call every time coins change or on open.</summary>
    public void Refresh(int playerCoins)
    {
        if (item == null) return;

        if (item.isUnlocked)
        {
            if (costText != null) costText.text = "";
            if (buyButtonLabel != null) buyButtonLabel.text = "✓ Desbloqueado";
            if (buyButton != null)
            {
                buyButton.interactable = false;
                SetButtonColor(unlockedColor);
            }
            return;
        }

        if (costText != null) costText.text = $"{item.lobbyCost} monedas";

        bool canAfford = playerCoins >= item.lobbyCost;

        if (buyButtonLabel != null) buyButtonLabel.text = "Comprar";
        if (buyButton != null)
        {
            buyButton.interactable = canAfford;
            SetButtonColor(canAfford ? lockedColor : noFundsColor);
        }
    }

    private void OnBuyClicked()
    {
        shopUI?.OnSlotPurchase(item);
    }

    private void SetButtonColor(Color c)
    {
        Image btnImg = buyButton?.GetComponent<Image>();
        if (btnImg != null) btnImg.color = c;
    }
}