using System.Collections.Generic;
using UnityEngine;

public class FirebaseTestButtons : MonoBehaviour
{
    public void LoginTest()
    {
        FirebaseAuthHandler.Instance.SignInAnonymously();
    }

    public void SaveTest()
    {
        PlayerSaveData data = new PlayerSaveData
        {
            currentHealth = 45,
            maxHealth = 100,
            coins = 25,
            permanentMoney = 10,
            roomsCompleted = 2,
            enemiesDefeated = 5,
            deaths = 1,
            inventory = new List<string> { "EspadaBase", "AmuletoRecuerdo" }
        };

        FirebaseSaveHandler.Instance.SavePlayerData(data);
    }

    public void LoadTest()
    {
        FirebaseSaveHandler.Instance.LoadPlayerData();
    }
}
