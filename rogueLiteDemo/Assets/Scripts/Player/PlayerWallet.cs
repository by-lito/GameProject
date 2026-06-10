using System;
using UnityEngine;

/// <summary>
/// Tracks both currencies. Persists across scenes via DontDestroyOnLoad.
/// Fires events so HUD updates automatically without polling.
///
/// AngelDust   = run currency (resets on death, drops from enemies via CoinPickup)
/// permanentMoney is legacy — lobby shop now uses PlayerController.coins instead.
/// </summary>
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet instance;

    [Header("RUN (Se pierde al morir)")]
    public int angelDust = 0;

    [Header("LOBBY (Permanente — legacy, usar PlayerController.coins)")]
    public int permanentMoney = 0;

    // HUD subscribes to this to update AngelDust display without polling
    public event Action<int> OnAngelDustChanged;

    void Awake()
    {
        // FIX: proper singleton + DontDestroyOnLoad
        // Previously had no null check → second scene would overwrite instance
        // and wallet was destroyed on scene change → all currency lost
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddAngelDust(int amount)
    {
        angelDust += amount;
        OnAngelDustChanged?.Invoke(angelDust);
        Debug.Log($"[Wallet] +{amount} AngelDust → Total: {angelDust}");
    }

    public void ResetRun()
    {
        angelDust = 0;
        OnAngelDustChanged?.Invoke(angelDust);
        Debug.Log("[Wallet] Run reseteada. AngelDust a 0.");
    }

    public bool CanAfford(int cost) => angelDust >= cost;

    public bool SpendDust(int cost)
    {
        if (angelDust < cost) return false;
        angelDust -= cost;
        OnAngelDustChanged?.Invoke(angelDust);
        return true;
    }

    // Legacy — kept for Firebase save compatibility
    public void AddEndLevelReward(int amount)
    {
        permanentMoney += amount;
        Debug.Log($"[Wallet] Recompensa de nivel: +{amount}. Total: {permanentMoney}");
    }

    public void SpendPermanentMoney(int amount)
    {
        permanentMoney -= amount;
    }
}