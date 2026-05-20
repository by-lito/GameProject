using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int currentHealth;
    public int maxHealth;
    public int coins;
    public int permanentMoney;
    public int roomsCompleted;
    public int enemiesDefeated;
    public int deaths;
    public List<string> inventory;

    public PlayerSaveData()
    {
        currentHealth = 0;
        maxHealth = 100;
        coins = 0;
        permanentMoney = 0;
        roomsCompleted = 0;
        enemiesDefeated = 0;
        deaths = 0;
        inventory = new List<string>();
    }
}
