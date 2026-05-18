using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseAuthHandler : MonoBehaviour
{
    public static FirebaseAuthHandler Instance { get; private set; }

    private FirebaseAuth auth;
    public FirebaseUser CurrentUser { get; private set; }

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

    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión anónima: " + task.Exception);
                return;
            }

            CurrentUser = task.Result.User;
            Debug.Log("Usuario autenticado correctamente. UID: " + CurrentUser.UserId);
        });
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