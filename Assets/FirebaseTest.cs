using UnityEngine;
using Firebase;
using Firebase.Firestore;

public class Firebasetest : MonoBehaviour
{
    FirebaseFirestore db;

    async void Start()
    {
        // Step 1 — Ensure Firebase dependencies are ready
        await FirebaseApp.CheckAndFixDependenciesAsync();

        // Step 2 — Get a Firestore instance
        db = FirebaseFirestore.DefaultInstance;

        Debug.Log("Firebase & Firestore initialized!");
    }
}

