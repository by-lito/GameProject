using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseSaveHandler : MonoBehaviour
{
    public static FirebaseSaveHandler Instance { get; private set; }

    private FirebaseFirestore db;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Firestore inicializado correctamente.");
    }

    public void SavePlayerData(PlayerSaveData data)
    {
        string userId = FirebaseAuthHandler.Instance.GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No se puede guardar: no hay usuario autenticado.");
            return;
        }

        Dictionary<string, object> playerData = new Dictionary<string, object>
        {
            { "currentHealth", data.currentHealth },
            { "maxHealth", data.maxHealth },
            { "coins", data.coins },
            { "permanentMoney", data.permanentMoney },
            { "roomsCompleted", data.roomsCompleted },
            { "enemiesDefeated", data.enemiesDefeated },
            { "deaths", data.deaths },
            { "inventory", data.inventory }
        };

        db.Collection("players").Document(userId).SetAsync(playerData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Datos del jugador guardados correctamente en Firestore.");
            }
            else
            {
                Debug.LogError("Error al guardar datos en Firestore: " + task.Exception);
            }
        });
    }

    public void LoadPlayerData()
    {
        string userId = FirebaseAuthHandler.Instance.GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No se puede cargar: no hay usuario autenticado.");
            return;
        }

        db.Collection("players").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompletedSuccessfully)
            {
                Debug.LogError("Error al cargar datos desde Firestore: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;

            if (!snapshot.Exists)
            {
                Debug.LogWarning("No hay datos guardados para este usuario.");
                return;
            }

            Debug.Log("Datos cargados correctamente desde Firestore.");

            int currentHealth = snapshot.GetValue<int>("currentHealth");
            int maxHealth = snapshot.GetValue<int>("maxHealth");
            int coins = snapshot.GetValue<int>("coins");
            int permanentMoney = snapshot.GetValue<int>("permanentMoney");
            int roomsCompleted = snapshot.GetValue<int>("roomsCompleted");
            int enemiesDefeated = snapshot.GetValue<int>("enemiesDefeated");
            int deaths = snapshot.GetValue<int>("deaths");

            Debug.Log("Vida actual: " + currentHealth);
            Debug.Log("Vida máxima: " + maxHealth);
            Debug.Log("Monedas: " + coins);
            Debug.Log("Dinero permanente: " + permanentMoney);
            Debug.Log("Salas completadas: " + roomsCompleted);
            Debug.Log("Enemigos derrotados: " + enemiesDefeated);
            Debug.Log("Muertes: " + deaths);
        });
    }
}