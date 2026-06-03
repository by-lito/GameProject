using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;

public class FirebaseAuthHandler : MonoBehaviour
{
    public static FirebaseAuthHandler Instance { get; private set; }

    private FirebaseAuth auth;
    public FirebaseUser CurrentUser { get; private set; }

    [Header("UI Control")]
    [SerializeField] private GameObject playButtonGO;

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
        auth = FirebaseAuth.DefaultInstance;
    }

    public void RegisterWithEmail(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("El email y la contraseña no pueden estar vacíos.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Registro cancelado.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Error al registrar usuario: " + task.Exception);
                return;
            }

            CurrentUser = task.Result.User;
            Debug.Log("Usuario registrado correctamente en Auth. UID: " + CurrentUser.UserId);

            // ¡AQUÍ ESTÁ EL CAMBIO CRUCIAL! 
            // En vez de cargar datos que no existen, creamos su nuevo documento en Firestore
            CreateNewPlayerData(CurrentUser.UserId);
        });
    }

    private void CreateNewPlayerData(string userId)
    {
        // Accedemos a Firestore directamente
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("players").Document(userId);

        // Estructura de datos idéntica a la que tienes en tu consola de Firebase
        Dictionary<string, object> defaultData = new Dictionary<string, object>
        {
            { "coins", 0 },
            { "currentHealth", 0 },
            { "deaths", 0 },
            { "enemiesDefeated", 0 },
            { "inventory", new List<string>() }, // Lista vacía para la EspadaBase, etc.
            { "maxHealth", 100 },
            { "permanentMoney", 0 },
            { "roomsCompleted", 0 }
        };

        Debug.Log("Creando documento en Firestore para el nuevo usuario...");

        docRef.SetAsync(defaultData).ContinueWithOnMainThread(firestoreTask =>
        {
            if (firestoreTask.IsFaulted || firestoreTask.IsCanceled)
            {
                Debug.LogError("Error al crear el documento en Firestore: " + firestoreTask.Exception);
                return;
            }

            Debug.Log("¡Documento de Firestore creado con éxito!");

            // ─────────────────────────────────────────────────────────
            // (Al registrarse con éxito, activamos el PLAY)
            if (playButtonGO != null)
            {
                playButtonGO.SetActive(true);
            }
            // ─────────────────────────────────────────────────────────

            // Ahora que el documento YA existe físicamente en la nube, 
            // llamamos a vuestro SaveHandler para que lo lea y actualice el HUD
            if (FirebaseSaveHandler.Instance != null)
            {
                FirebaseSaveHandler.Instance.LoadPlayerData();
            }
        });
    }

    public void LoginWithEmail(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("El email y la contraseña no pueden estar vacíos.");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión: " + task.Exception);
                return;
            }

            CurrentUser = task.Result.User;
            Debug.Log("Usuario inició sesión correctamente. UID: " + CurrentUser.UserId);
            Debug.Log("Email: " + CurrentUser.Email);

            // ─────────────────────────────────────────────────────────
            // ¡AQUÍ TAMBIÉN VA! (Al loguearse bien, activamos el PLAY)
            if (playButtonGO != null)
            {
                playButtonGO.SetActive(true);
            }
            // ─────────────────────────────────────────────────────────

            if (FirebaseSaveHandler.Instance != null)
            {
                FirebaseSaveHandler.Instance.LoadPlayerData();
            }
        });
    }

    public void Logout()
    {
        auth.SignOut();
        CurrentUser = null;
        Debug.Log("Sesión cerrada correctamente.");
    }

    public string GetUserId()
    {
        if (CurrentUser == null)
        {
            Debug.LogWarning("No hay usuario autenticado.");
            return null;
        }

        return CurrentUser.UserId;
    }
}