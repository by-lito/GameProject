using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Configuracion Base")]
    public float maxHP = 100f;
    public float currentHP;

    /// <summary>
    /// Set to TRUE on bosses (BossPhase1) to prevent Health.Die() from calling
    /// Destroy(gameObject) automatically. The boss manages its own destruction
    /// via HandleDeath → PhaseTransition coroutine.
    /// Leave FALSE on regular enemies — they are destroyed normally.
    /// </summary>
    [Header("Boss Setting")]
    public bool manageOwnDestruction = false;

    public Action<float> OnDamage;
    public Action<float> OnHeal;
    public Action OnDeath;

    void Awake()
    {
        currentHP = gameObject.CompareTag("Player") ? 0f : maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (gameObject.CompareTag("Player"))
        {
            currentHP += amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnDamage?.Invoke(amount);
            Debug.Log($"{gameObject.name} - Purificación: {currentHP} / {maxHP}");

            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetTrigger("isHit");

            if (currentHP >= maxHP) Die();
        }
        else
        {
            currentHP -= amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnDamage?.Invoke(amount);
            Debug.Log($"{gameObject.name} - Vida restante: {currentHP}");
            if (currentHP <= 0) Die();
        }
    }

    public void Heal(float amount)
    {
        if (gameObject.CompareTag("Player"))
        {
            currentHP -= amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnHeal?.Invoke(amount);
        }
        else
        {
            currentHP += amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnHeal?.Invoke(amount);
        }
    }

    public void Die()
    {
        OnDeath?.Invoke(); // EnemyBase.HandleDeath / BossPhase1.HandleDeath subscribe here

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Has resucitado por completo... Volviendo al lobby.");
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetTrigger("isDead");
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null) controller.enabled = false;
            StartCoroutine(ReloadLobbyDelayed());
            return;
        }

        // FIX: Removed AngelDust drop from here — EnemyBase.HandleDeath drops it.
        //      Two systems dropping dust caused double rewards on every kill.

        // FIX: Only auto-destroy if the enemy does not manage its own destruction.
        //      Bosses set manageOwnDestruction = true so their PhaseTransition
        //      coroutine can complete before the object is destroyed.
        if (!manageOwnDestruction)
            Destroy(gameObject);
    }

    private System.Collections.IEnumerator ReloadLobbyDelayed()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby_3D");
        Destroy(gameObject);
    }
}