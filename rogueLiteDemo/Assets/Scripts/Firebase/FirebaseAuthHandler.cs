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

    // ─────────────────────────────────────────────────────────────────
    // LA CASILLA DONDE COLECTAREMOS TU TEXTO AMARILLO
    // ─────────────────────────────────────────────────────────────────
    [Header("Configuración Emergencia Inicio de Sesión")]
    [SerializeField] private GameObject textoAvisoL_GO; 
    private float timer = 0f;
    private bool mensajeMostrado = false;
    // ─────────────────────────────────────────────────────────────────

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

        // Al empezar, apagamos el cartel para que el menú parezca normal
        if (textoAvisoL_GO != null)
        {
            textoAvisoL_GO.SetActive(false);
        }
    }

    private void Update()
    {
        // Si pasan 3 segundos colgado sin internet ni usuario, activamos tu cartel amarillo
        if (!mensajeMostrado && CurrentUser == null)
        {
            timer += Time.deltaTime;
            if (timer >= 3f)
            {
                mensajeMostrado = true;
                
                if (textoAvisoL_GO != null)
                {
                    textoAvisoL_GO.SetActive(true);
                    Debug.LogWarning("Modo seguro: Activando cartel estático de la Tecla F1.");
                }
            }
        }

        // Si pulsas 'F1', saltas directamente a la demo del juego
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.LogWarning("¡Bypass activado! Saltando al Lobby.");

            if (playButtonGO != null)
            {
                playButtonGO.SetActive(true);
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Lobby_3D");
        }
    }

    public void RegisterWithEmail(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;
            CurrentUser = task.Result.User;
            CreateNewPlayerData(CurrentUser.UserId);
        });
    }

    private void CreateNewPlayerData(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("players").Document(userId);

        Dictionary<string, object> defaultData = new Dictionary<string, object>
        {
            { "coins", 0 },
            { "currentHealth", 100 }, 
            { "deaths", 0 },
            { "enemiesDefeated", 0 },
            { "inventory", new List<string>() }, 
            { "maxHealth", 100 },
            { "permanentMoney", 0 },
            { "roomsCompleted", 0 }
        };

        docRef.SetAsync(defaultData).ContinueWithOnMainThread(firestoreTask =>
        {
            if (firestoreTask.IsFaulted || firestoreTask.IsCanceled) return;
            if (playButtonGO != null) playButtonGO.SetActive(true);
            if (FirebaseSaveHandler.Instance != null) FirebaseSaveHandler.Instance.LoadPlayerData();
        });
    }

    public void LoginWithEmail(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;
            CurrentUser = task.Result.User;
            if (playButtonGO != null) playButtonGO.SetActive(true);
            if (FirebaseSaveHandler.Instance != null) FirebaseSaveHandler.Instance.LoadPlayerData();
        });
    }

    public void Logout()
    {
        auth.SignOut();
        CurrentUser = null;
    }

    public string GetUserId()
    {
        if (CurrentUser == null) return null;
        return CurrentUser.UserId;
    }
}