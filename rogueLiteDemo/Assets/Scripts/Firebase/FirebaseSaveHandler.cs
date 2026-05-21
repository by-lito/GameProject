using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;


public class FirebaseSaveHandler : MonoBehaviour
{
    public static FirebaseSaveHandler Instance { get; private set; }

    // Asigna aquí los textos de tu HUD
    public TMP_Text VidasText;
    public TMP_Text MonedasText;
    public TMP_Text SalasText;

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
        LoadPlayerData();
    }

    public void SavePlayerData(PlayerSaveData data)
    {
        string userId = FirebaseAuthHandler.Instance.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No se puede guardar: no hay usuario autenticado.");
            return;
        }

        DocumentReference docRef = db.Collection("players").Document(userId);
        docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Datos del jugador guardados correctamente en Firestore.");
            else
                Debug.LogError("Error al guardar datos en Firestore: " + task.Exception);
        });
    }

    public void LoadPlayerData()
    {
        if (FirebaseAuthHandler.Instance == null)
        {
            Debug.LogError("FirebaseAuthHandler no inicializado.");
            return;
        }

        string userId = FirebaseAuthHandler.Instance.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No se puede cargar: no hay usuario autenticado.");
            return;
        }

        DocumentReference docRef = db.Collection("players").Document(userId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    PlayerSaveData data = snapshot.ConvertTo<PlayerSaveData>();
                    UpdateHUD(data);
                    Debug.Log("Datos cargados correctamente desde Firestore.");
                }
                else
                {
                    Debug.Log("No hay datos guardados, creando datos por defecto.");
                    PlayerSaveData defaultData = new PlayerSaveData
                    {
                        currentHealth = 100,
                        maxHealth = 100,
                        coins = 0,
                        roomsCompleted = 0
                    };
                    UpdateHUD(defaultData);
                }
            }
            else
            {
                Debug.LogError("Error al cargar datos desde Firestore: " + task.Exception);
            }
        });
    }

    public void UpdateHUD(PlayerSaveData playerData)
    {
        if (playerData != null)
        {
            VidasText.text = "Vidas: " + playerData.currentHealth;
            MonedasText.text = "Monedas: " + playerData.coins;
            SalasText.text = "Salas completadas: " + playerData.roomsCompleted;
        }
    }
}