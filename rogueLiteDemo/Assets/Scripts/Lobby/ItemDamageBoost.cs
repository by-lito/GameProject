using UnityEngine;

/// <summary>
/// Lobby Item 2: Damage Boost.
/// Inspector: itemName="Alma de Guerrero", lobbyCost=40, icon=your_icon_sprite
/// </summary>
public class ItemDamageBoost : LobbyUnlockableItem
{
    [Header("Effect")]
    public float damageBonus = 5f;

    public override void OnEquip(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.meleeDamage += damageBonus;
            Debug.Log($"[ItemDamageBoost] Damage +{damageBonus}. New damage: {pc.meleeDamage}");
        }
    }
}