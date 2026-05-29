using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
[System.Serializable]
public class PlayerSaveData
{
    [FirestoreProperty]
    public int currentHealth { get; set; }

    [FirestoreProperty]
    public int maxHealth { get; set; }

    [FirestoreProperty]
    public int coins { get; set; }

    [FirestoreProperty]
    public int permanentMoney { get; set; }

    [FirestoreProperty]
    public int roomsCompleted { get; set; }

    [FirestoreProperty]
    public int enemiesDefeated { get; set; }

    [FirestoreProperty]
    public int deaths { get; set; }

    [FirestoreProperty]
    public List<string> inventory { get; set; }
}