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
            Debug.Log("Usuario registrado correctamente. UID: " + CurrentUser.UserId);
            Debug.Log("Email: " + CurrentUser.Email);
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
            if (task.IsCanceled)
            {
                Debug.LogError("Inicio de sesión cancelado.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión: " + task.Exception);
                return;
            }

            CurrentUser = task.Result.User;
            Debug.Log("Usuario inició sesión correctamente. UID: " + CurrentUser.UserId);
            Debug.Log("Email: " + CurrentUser.Email);
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