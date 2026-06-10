using UnityEngine;

/// <summary>
/// Base for all 4 lobby-unlockable items.
/// Persists unlock state via PlayerPrefs.
/// </summary>
public abstract class LobbyUnlockableItem : Item
{
    private string PrefKey => "lobby_item_unlocked_" + itemName;

    // FIX: Changed Start() to Awake() so isUnlocked is set before
    // RunItemPickup.Start() checks it. Awake always runs before Start.
    void Awake()
    {
        isUnlocked = PlayerPrefs.GetInt(PrefKey, 0) == 1;
    }

    public override void OnUnlock()
    {
        isUnlocked = true;
        PlayerPrefs.SetInt(PrefKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"[LobbyShop] {itemName} desbloqueado permanentemente.");
    }

    /// <summary>
    /// Called by LobbyShopUI. Spends PlayerController.coins (permanent currency).
    /// </summary>
    public bool TryBuy(PlayerController buyer)
    {
        if (isUnlocked)
        {
            Debug.Log($"[LobbyShop] {itemName} ya está desbloqueado.");
            return false;
        }

        if (buyer.coins < lobbyCost)
        {
            Debug.Log($"[LobbyShop] Monedas insuficientes. Necesitas {lobbyCost}, tienes {buyer.coins}.");
            return false;
        }

        buyer.coins -= lobbyCost;
        buyer.ActualizarHUDLocal();
        OnUnlock();
        return true;
    }
}