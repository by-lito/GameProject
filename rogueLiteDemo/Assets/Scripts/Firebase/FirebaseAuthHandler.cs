using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseAuthHandler : MonoBehaviour
{
    public static FirebaseAuthHandler Instance { get; private set; }

    [Header("Campos de UI")]
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text statusText;   
    [SerializeField] private GameObject playButton; 
    [SerializeField] private GameObject rankingButton;

    private FirebaseAuth auth;
    public FirebaseUser CurrentUser { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playButton != null) playButton.SetActive(false);
        if (rankingButton != null) rankingButton.SetActive(false);
        SetStatus("");
    }

    private FirebaseAuth Auth => auth ??= FirebaseAuth.DefaultInstance;

    public void OnRegisterClicked() => Register(emailField.text.Trim(), passwordField.text);
    public void OnLoginClicked()    => Login(emailField.text.Trim(), passwordField.text);

    public void Register(string email, string password)
    {
        if (!ValidInput(email, password)) return;
        SetStatus("Creando cuenta...");

        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) { SetStatus(TranslateError(task.Exception)); return; }

            CurrentUser = task.Result.User;
            if (StatsTracker.Instance != null) StatsTracker.Instance.CreateNewUser(CurrentUser.UserId, email);
            OnAuthSuccess("Cuenta creada. ¡Bienvenido!");
        });
    }

    public void Login(string email, string password)
    {
        if (!ValidInput(email, password)) return;
        SetStatus("Entrando...");

        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) { SetStatus(TranslateError(task.Exception)); return; }

            CurrentUser = task.Result.User;
            if (StatsTracker.Instance != null) StatsTracker.Instance.SetUser(CurrentUser.UserId);
            OnAuthSuccess("Sesión iniciada.");
        });
    }

    public void Logout()
    {
        Auth.SignOut();
        CurrentUser = null;
        if (playButton != null) playButton.SetActive(false);
        if (rankingButton != null) rankingButton.SetActive(false);
    }

    public string GetUserId() => CurrentUser?.UserId;

    private void OnAuthSuccess(string msg)
    {
        SetStatus(msg);
        if (playButton != null) playButton.SetActive(true);
        if (rankingButton != null) rankingButton.SetActive(true);
    }

    private bool ValidInput(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Rellena email y contraseña."); return false;
        }
        if (password.Length < 6)
        {
            SetStatus("La contraseña debe tener al menos 6 caracteres."); return false;
        }
        return true;
    }

    private void SetStatus(string msg) { if (statusText != null) statusText.text = msg; }

    private string TranslateError(System.AggregateException ex)
    {
        var fbEx = ex?.GetBaseException() as Firebase.FirebaseException;
        if (fbEx == null) return "Error desconocido.";

        switch ((AuthError)fbEx.ErrorCode)
        {
            case AuthError.EmailAlreadyInUse: return "Ese email ya está registrado.";
            case AuthError.InvalidEmail:      return "El email no es válido.";
            case AuthError.WrongPassword:     return "Contraseña incorrecta.";
            case AuthError.UserNotFound:      return "No existe una cuenta con ese email.";
            case AuthError.WeakPassword:      return "La contraseña es demasiado débil.";
            case AuthError.MissingPassword:   return "Falta la contraseña.";
            default:                          return "Error: " + fbEx.Message;
        }
    }
}