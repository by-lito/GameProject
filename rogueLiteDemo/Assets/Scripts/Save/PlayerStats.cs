using Firebase.Firestore;

[FirestoreData]
public class PlayerStats
{
    [FirestoreProperty] public string email { get; set; }
    [FirestoreProperty] public int roomsCompleted { get; set; }
    [FirestoreProperty] public int totalMoney { get; set; }
    [FirestoreProperty] public int enemiesDefeated { get; set; }
    [FirestoreProperty] public int deaths { get; set; }
    [FirestoreProperty] public int runsCompleted { get; set; }
}