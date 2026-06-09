using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;

public class RankingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject rowPrefab;     
    [SerializeField] private Transform contentParent;  
    [SerializeField] private TMP_Text statusText;      
    [SerializeField] private int topCount = 10;

    private FirebaseFirestore db;
    private FirebaseFirestore Db => db ??= FirebaseFirestore.DefaultInstance;

    public void OpenRanking()
    {
        if (rankingPanel != null) rankingPanel.SetActive(true);
        LoadRanking();
    }

    public void CloseRanking()
    {
        if (rankingPanel != null) rankingPanel.SetActive(false);
    }

    private void ClearRows()
    {
        if (contentParent == null) return;
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
    }

    private void LoadRanking()
    {
        ClearRows();
        if (statusText != null) { statusText.gameObject.SetActive(true); statusText.text = "Cargando..."; }

        Db.Collection("players")
          .OrderByDescending("runsCompleted")
          .Limit(topCount)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                if (statusText != null) statusText.text = "Error al cargar el ranking.";
                Debug.LogError("[Ranking] " + task.Exception);
                return;
            }

            QuerySnapshot snapshot = task.Result;
            if (snapshot.Count == 0)
            {
                if (statusText != null) statusText.text = "Todavía no hay datos.";
                return;
            }

            if (statusText != null) statusText.gameObject.SetActive(false);

            int pos = 1;
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                PlayerStats s = doc.ConvertTo<PlayerStats>();
                GameObject row = Instantiate(rowPrefab, contentParent);
                row.GetComponent<RankingRow>().Setup(pos, s);
                pos++;
            }
        });
    }
}