using UnityEngine;
public class BossArenaController : MonoBehaviour
{
    [Header("Referencias de la sala")]
    [Tooltip("Componente Health del boss de esta sala.")]
    [SerializeField] private Health bossHealth;

    [Tooltip("GameObject del portal de salida. Permanecerá desactivado hasta que el boss muera.")]
    [SerializeField] private GameObject exitPortal;

    private bool subscribed = false;

    private void Awake()
    {
        if (bossHealth == null)
        {
            Debug.LogError(
                "[BossArenaController] No se ha asignado 'Boss Health'. " +
                "Arrastra el boss desde la Hierarchy al campo del Inspector.",
                this
            );
        }

        if (exitPortal == null)
        {
            Debug.LogError(
                "[BossArenaController] No se ha asignado 'Exit Portal'. " +
                "Arrastra el ExitPortal3D desde la Hierarchy al campo del Inspector.",
                this
            );
            return;
        }

        if (exitPortal.activeSelf)
        {
            Debug.LogWarning(
                "[BossArenaController] El portal estaba activo al iniciar la escena. " +
                "Lo desactivamos hasta que muera el boss.",
                this
            );
            exitPortal.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (bossHealth != null && !subscribed)
        {
            bossHealth.OnDeath += HandleBossDefeated;
            subscribed = true;
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null && subscribed)
        {
            bossHealth.OnDeath -= HandleBossDefeated;
            subscribed = false;
        }
    }
    private void HandleBossDefeated()
    {
        if (exitPortal == null) return;

        exitPortal.SetActive(true);
        Debug.Log("[BossArenaController] Boss derrotado. Portal de salida activado.");
    }
}