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

        Debug.Log("Buscando documento de usuario: " + userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Entrando al callback de Firestore");

            Debug.Log("Task completada: " + task.IsCompleted);
            Debug.Log("Task faulted: " + task.IsFaulted);

            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                Debug.Log("Snapshot existe: " + snapshot.Exists);

                if (snapshot.Exists)
                {
                    try
                    {
                        PlayerSaveData data = snapshot.ConvertTo<PlayerSaveData>();

                        Debug.Log("Conversión realizada");
                        Debug.Log("Vida: " + data.currentHealth);
                        Debug.Log("Monedas: " + data.coins);
                        Debug.Log("Salas: " + data.roomsCompleted);

                        UpdateHUD(data);
                        Debug.Log("Datos cargados correctamente desde Firestore.");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("ERROR CONVERT_TO: " + e);
                    }
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
        Debug.Log("UPDATE HUD");

        if (playerData != null)
        {
            HUDController.Instance.SetVidas(playerData.currentHealth);
            HUDController.Instance.SetMonedas(playerData.coins);
            HUDController.Instance.SetSalas(playerData.roomsCompleted);
        }
    }
}