using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static bool IsReady { get; private set; }

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                IsReady = true;
                Debug.Log("[Firebase] Listo.");
            }
            else
            {
                Debug.LogError("[Firebase] No disponible: " + task.Result);
            }
        });
    }
}