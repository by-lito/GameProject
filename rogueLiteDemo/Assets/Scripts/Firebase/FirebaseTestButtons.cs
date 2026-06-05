using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseTestButtons : MonoBehaviour
{
    [Header("Campos de autenticación")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    // Referencia directa al botón de Play del menú
    [SerializeField] private GameObject playButtonDeEstaEscena;

    public void RegisterTest()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        FirebaseAuthHandler.Instance.RegisterWithEmail(email, password);
    }

    public void LoginTest()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Llamamos al login de Firebase que ya funciona perfectamente
        if (FirebaseAuthHandler.Instance != null)
        {
            FirebaseAuthHandler.Instance.LoginWithEmail(email, password);

            // Activamos el botón de PLAY inmediatamente en la escena al pulsar el login correcto
            if (playButtonDeEstaEscena != null)
            {
                playButtonDeEstaEscena.SetActive(true);
            }
        }
    }

    public void LogoutTest()
    {
        FirebaseAuthHandler.Instance.Logout();
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

    public void PlayIfLoggedIn()
    {
        if (FirebaseAuthHandler.Instance != null &&
            FirebaseAuthHandler.Instance.CurrentUser != null)
        {
            GameObject canvas = GameObject.Find("Canvas");

            if (canvas != null)
            {
                Destroy(canvas);
            }

            FindAnyObjectByType<SceneLoader>().LoadLobby();
        }
        else
        {
            Debug.LogWarning("Debes iniciar sesión antes de jugar.");
        }
    }

}