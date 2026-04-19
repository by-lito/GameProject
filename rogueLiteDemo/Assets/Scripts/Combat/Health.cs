using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHP;
    public float currentHP;

    public Action<float> OnDamage;
    public Action<float> OnHeal;
    public Action OnDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnDamage?.Invoke(amount);

        if (currentHP <= 0)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnHeal?.Invoke(amount);
    }
public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}