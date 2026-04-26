using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus status = task.Result;

            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase funciona correctamente");
            }
            else
            {
                Debug.LogError("Firebase no funciona: " + status);
            }
        });
    }
}