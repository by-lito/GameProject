using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingRow : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text detailsText;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(Toggle);
        if (detailsText != null) detailsText.gameObject.SetActive(false);
    }

    public void Setup(int rank, PlayerStats s)
    {
        string nombre = string.IsNullOrEmpty(s.email) ? "(sin nombre)" : s.email;
        headerText.text = $"#{rank}   {nombre}   —   {s.runsCompleted} runs";
        detailsText.text =
            $"Runs terminadas: {s.runsCompleted}\n" +
            $"Salas completadas: {s.roomsCompleted}\n" +
            $"Enemigos derrotados: {s.enemiesDefeated}\n" +
            $"Dinero total: {s.totalMoney}\n" +
            $"Muertes: {s.deaths}";
        detailsText.gameObject.SetActive(false);
    }

    private void Toggle()
    {
        if (detailsText != null)
            detailsText.gameObject.SetActive(!detailsText.gameObject.activeSelf);
    }
}