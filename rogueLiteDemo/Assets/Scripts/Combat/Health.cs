using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Configuracion Base")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Ajustes de Recompensa (Para Enemigos)")]
    public int dustReward = 15;

    public Action<float> OnDamage;
    public Action<float> OnHeal;
    public Action OnDeath;

    void Awake()
    {
        // El jugador empieza en 0 (corrupción máxima, barra vacía)
        if (gameObject.CompareTag("Player"))
        {
            currentHP = 0f;
        }
        else
        {
            // Los enemigos siguen empezando con la vida llena
            currentHP = maxHP;
        }
    }

    public void TakeDamage(float amount)
    {
        if (gameObject.CompareTag("Player"))
        {
            // Al jugador la "curación" enemiga le SUMA puntos a la barra
            currentHP += amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            OnDamage?.Invoke(amount);
            Debug.Log(gameObject.name + " - Purificación actual: " + currentHP + " / " + maxHP);

            // [NUEVO] Lanza el trigger de daño en el Animator de Aurora
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetTrigger("isHit");

            // Si llega al máximo de vida (purificado), muere
            if (currentHP >= maxHP)
            {
                Die();
            }
        }
        else
        {
            // Lógica original intacta para los enemigos (perder vida)
            currentHP -= amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            OnDamage?.Invoke(amount);
            Debug.Log(gameObject.name + " - Vida/Vitalidad restante: " + currentHP);

            if (currentHP <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(float amount)
    {
        if (gameObject.CompareTag("Player"))
        {
            // Para el jugador, "curarse" (hacerse daño a sí mismo) le BAJA la barra
            currentHP -= amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnHeal?.Invoke(amount);
        }
        else
        {
            // Lógica original para enemigos
            currentHP += amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            OnHeal?.Invoke(amount);
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();

        // si es un enemigo y muere, da Angel Dust (Intacto)
        if (gameObject.CompareTag("Enemy"))
        {
            if (PlayerWallet.instance != null)
            {
                PlayerWallet.instance.AddAngelDust(dustReward);
                Debug.Log("¡Enemigo destruido! Recibes " + dustReward + " de polvo.");
            }
            Destroy(gameObject); // Destrucción inmediata solo para enemigos
        }

        // si es el Jugador y muere (llega a 100% de purificación)
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Has resucitado por completo... Volviendo al lobby.");

            //Lanza el trigger de muerte en el Animator de Aurora
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetTrigger("isDead");

            //Desactiva el movimiento para que no se desplace mientras cae
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null) controller.enabled = false;

            // Espera 2 segundos para que se vea el Blend Tree de muerte antes de cambiar de escena
            StartCoroutine(ReloadLobbyDelayed());
        }
    }

    //Corrutina para dar margen a que termine la animación de muerte
    private System.Collections.IEnumerator ReloadLobbyDelayed()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
        Destroy(gameObject);
    }
}