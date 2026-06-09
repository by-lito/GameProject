using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

public class StatsTracker : MonoBehaviour
{
    public static StatsTracker Instance { get; private set; }

    private FirebaseFirestore db;
    private string userId;

    private int dRooms, dMoney, dEnemies, dDeaths, dRuns;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private FirebaseFirestore Db => db ??= FirebaseFirestore.DefaultInstance;

    public void CreateNewUser(string uid, string email)
    {
        userId = uid;
        ResetDeltas();

        PlayerStats stats = new PlayerStats { email = email };
        Db.Collection("players").Document(uid).SetAsync(stats).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("[Stats] Error creando documento: " + t.Exception);
            else Debug.Log("[Stats] Documento creado para " + email);
        });
    }

    public void SetUser(string uid)
    {
        userId = uid;
        ResetDeltas();
    }

    public void AddRoomCompleted()   => dRooms++;
    public void AddMoney(int amount) => dMoney += amount;
    public void AddEnemyDefeated()   => dEnemies++;
    public void AddDeath()           => dDeaths++;
    public void AddRunCompleted()    => dRuns++;

    public void Flush()
    {
        if (string.IsNullOrEmpty(userId)) return;
        if (dRooms == 0 && dMoney == 0 && dEnemies == 0 && dDeaths == 0 && dRuns == 0) return;

        var updates = new Dictionary<string, object>
        {
            { "roomsCompleted",  FieldValue.Increment(dRooms) },
            { "totalMoney",      FieldValue.Increment(dMoney) },
            { "enemiesDefeated", FieldValue.Increment(dEnemies) },
            { "deaths",          FieldValue.Increment(dDeaths) },
            { "runsCompleted",   FieldValue.Increment(dRuns) },
        };

        Db.Collection("players").Document(userId).UpdateAsync(updates).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("[Stats] Error al volcar: " + t.Exception);
            else Debug.Log("[Stats] Stats subidas.");
        });

        ResetDeltas();
    }

    private void ResetDeltas() => dRooms = dMoney = dEnemies = dDeaths = dRuns = 0;
    private void OnApplicationQuit() => Flush();
}