using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                Debug.Log(" Firebase initialized successfully!");
            }
            else
            {
                Debug.LogError(" Could not initialize Firebase: " + status);
            }
        });
    }
}

