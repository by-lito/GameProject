using System;
using UnityEngine;

/// <summary>
/// Place in lobby or run scenes. When player walks into the trigger zone,
/// automatically opens the MemoryUI with this memory's painting.
/// No purchase needed — memories are permanent from the start.
///
/// PREFAB STRUCTURE:
///   MemoryObject
///     ├── SphereCollider      (Is Trigger = true, radius ~1.5)
///     ├── MemoryPickup        (this script)
///     ├── SpriteRenderer      (small icon visible in world)
///     └── Billboard
///
/// SCENE SETUP:
///   - One MemoryUI canvas in the scene (assign to memoryUI field)
///   - Assign memoryPainting (your pixel art sprite)
///   - Assign memoryTitle
/// </summary>
public class MemoryPickup : MonoBehaviour
{
    [Header("Memory Content")]
    public Sprite memoryPainting;       // Your pixel art sprite
    [TextArea(1, 3)]
    public string memoryTitle = "Recuerdo";

    [Header("UI")]
    public MemoryUI memoryUI;           // Shared canvas in the scene

    private bool opened = false;

    void OnTriggerEnter(Collider other)
    {
        if (opened) return;
        if (!other.CompareTag("Player")) return;

        opened = true;

        if (memoryUI != null)
            memoryUI.Show(memoryPainting, memoryTitle);
        else
            Debug.LogWarning("[MemoryPickup] memoryUI not assigned.");
    }
}