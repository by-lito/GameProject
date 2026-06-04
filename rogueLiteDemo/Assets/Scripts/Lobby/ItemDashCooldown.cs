using UnityEngine;

/// <summary>
/// Lobby Item 3: Reduced Dash Cooldown.
/// Inspector: itemName="Torbellino", lobbyCost=35, icon=your_icon_sprite
/// </summary>
public class ItemDashCooldown : LobbyUnlockableItem
{
    [Header("Effect")]
    public float cooldownReduction = 0.4f;

    public override void OnEquip(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.dashCooldown = Mathf.Max(0.1f, pc.dashCooldown - cooldownReduction);
            Debug.Log($"[ItemDashCooldown] Dash cooldown reduced by {cooldownReduction}. New: {pc.dashCooldown}");
        }
    }
}