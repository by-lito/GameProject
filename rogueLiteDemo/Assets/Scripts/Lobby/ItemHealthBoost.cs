using UnityEngine;

/// <summary>
/// Lobby Item 4: Extra Health.
/// Inspector: itemName="Voluntad de Hierro", lobbyCost=50, icon=your_icon_sprite
/// </summary>
public class ItemHealthBoost : LobbyUnlockableItem
{
    [Header("Effect")]
    public float healthBonus = 25f;

    public override void OnEquip(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.currentHealth = Mathf.Min(pc.currentHealth + healthBonus, 100f);
            pc.ActualizarHUDLocal();
            Debug.Log($"[ItemHealthBoost] Health +{healthBonus}. Current: {pc.currentHealth}");
        }
    }
}