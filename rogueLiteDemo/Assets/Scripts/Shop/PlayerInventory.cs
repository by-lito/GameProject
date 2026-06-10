using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> currentItems = new List<Item>();
    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary>
    /// Adds item to inventory and applies its effect to the player.
    /// FIX: was passing PlayerInventory's own gameObject to OnEquip, which has
    /// no PlayerController/Health. Now finds the actual Player tagged object.
    /// </summary>
    public void AddItem(Item newItem)
    {
        if (newItem == null) return;

        currentItems.Add(newItem);
        Debug.Log($"[Inventario] Objeto añadido: {newItem.itemName}");

        // FIX: pass the Player, not this inventory object
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            newItem.OnEquip(player);
        else
            Debug.LogError("[PlayerInventory] No se encontró el jugador (tag 'Player') al equipar.");
    }

    /// <summary>Call on run start/reset to wipe run-only items.</summary>
    public void ClearRunItems()
    {
        currentItems.Clear();
        Debug.Log("[Inventario] Inventario de run limpiado.");
    }
}