using UnityEngine;

/// <summary>
/// Lobby Item 1: Speed Boost.
/// Inspector: itemName="Botas Aéreas", lobbyCost=30, icon=your_icon_sprite
/// </summary>
public class ItemSpeedBoost : LobbyUnlockableItem
{
    [Header("Effect")]
    public float speedBonus = 1.5f;

    public override void OnEquip(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.moveSpeed += speedBonus;
            Debug.Log($"[ItemSpeedBoost] Speed +{speedBonus}. New speed: {pc.moveSpeed}");
        }
    }
}