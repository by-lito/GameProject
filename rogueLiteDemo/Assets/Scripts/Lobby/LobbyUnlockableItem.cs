using UnityEngine;

/// <summary>
/// Base class for all 4 lobby-unlockable items.
/// Persists unlock state via PlayerPrefs.
/// Attach to a child GameObject of LobbyItemsHolder in the Lobby scene.
/// No renderer or collider needed — these are data objects, not world objects.
/// </summary>
public abstract class LobbyUnlockableItem : Item
{
    // Unique key per item for PlayerPrefs
    private string PrefKey => "lobby_item_unlocked_" + itemName;

    void Start()
    {
        // Restore persisted unlock state on scene load
        isUnlocked = PlayerPrefs.GetInt(PrefKey, 0) == 1;
    }

    public override void OnUnlock()
    {
        base.OnUnlock(); // sets isUnlocked = true
        PlayerPrefs.SetInt(PrefKey, 1);
        PlayerPrefs.Save();
    }

    /// <summary>Called by LobbyShopUI to attempt purchase.</summary>
    public bool TryBuy(PlayerController buyer)
    {
        if (isUnlocked)
        {
            Debug.Log($"[LobbyShop] {itemName} already unlocked.");
            return false;
        }

        if (buyer.coins < lobbyCost)
        {
            Debug.Log($"[LobbyShop] Not enough coins. Need {lobbyCost}, have {buyer.coins}.");
            return false;
        }

        buyer.coins -= lobbyCost;
        buyer.ActualizarHUDLocal();
        OnUnlock();
        Debug.Log($"[LobbyShop] Purchased {itemName}!");
        return true;
    }
}
