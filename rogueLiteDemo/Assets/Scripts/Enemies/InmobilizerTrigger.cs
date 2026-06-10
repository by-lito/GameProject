using UnityEngine;

/// <summary>
/// Attach to the TriggerZone child of EnemyInmobilizer.
/// The child needs: SphereCollider with Is Trigger = true.
/// This keeps the parent collider as non-trigger (for physics),
/// while still detecting player contact.
///
/// SETUP:
///   EnemyInmobilizerRoot
///     ├── CapsuleCollider  (Is Trigger = false — blocks walls/floors)
///     ├── EnemyInmobilizer
///     └── TriggerZone      ← add this child
///           ├── SphereCollider  (Is Trigger = TRUE, Radius = 0.6)
///           └── InmobilizerTrigger  (this script)
/// </summary>
public class InmobilizerTrigger : MonoBehaviour
{
    private EnemyInmobilizer parent;

    void Awake()
    {
        parent = GetComponentInParent<EnemyInmobilizer>();
        if (parent == null)
            Debug.LogError("[InmobilizerTrigger] No EnemyInmobilizer found in parent.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        parent?.OnPlayerContact(other.gameObject);
    }
}