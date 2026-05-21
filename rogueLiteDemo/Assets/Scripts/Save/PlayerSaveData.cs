using System.Collections.Generic;

[System.Serializable]
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
}