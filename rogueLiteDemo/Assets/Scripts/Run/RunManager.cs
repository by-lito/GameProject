using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public enum RoomKind { Combat, Shop, Boss }

    [Header("Guion de la run (en orden, tras salir del Lobby)")]
    [Tooltip("Por defecto: Combate, Combate, Tienda, Boss.")]
    [SerializeField]
    private List<RoomKind> runSequence = new List<RoomKind>
    {
        RoomKind.Combat, RoomKind.Combat, RoomKind.Shop, RoomKind.Boss
    };

    [Header("Pool de salas de combate")]
    [Tooltip("Nombres de escena de combate. Se elige una al azar para cada hueco de combate.")]
    [SerializeField]
    private List<string> combatPool = new List<string> { "Room_Combat_01", "Room_Combat_02" };

    [Header("Salas fijas")]
    [SerializeField] private string shopScene = "Room_Shop_01";
    [SerializeField] private string bossScene = "Room_Boss_01";
    [SerializeField] private string lobbyScene = "Lobby_3D";
    [SerializeField] private string endDemoScene = "EndDemo";

    private int currentIndex = -1;      
    private string lastCombatScene = ""; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[RunManager] Listo. Salas en la run: " + runSequence.Count);
    }

    public void StartRun()
    {
        currentIndex = -1;
        lastCombatScene = "";

        if (PlayerWallet.instance != null)
            PlayerWallet.instance.ResetRun(); 

        if (GameManager.Instance != null)
            GameManager.Instance.StartRun(); 

        LoadNextRoom();
    }

    public void LoadNextRoom()
    {
        currentIndex++;

        if (currentIndex >= runSequence.Count)
        {
            CompleteRun();
            return;
        }

        string scene = SceneForSlot(runSequence[currentIndex]);
        Debug.Log("[RunManager] Sala " + (currentIndex + 1) + "/" + runSequence.Count + " -> " + scene);
        LoadScene(scene);
    }

    private void CompleteRun()
    {
        Debug.Log("[RunManager] Run completada. Pantalla de fin.");
        LoadScene(endDemoScene);
    }

    private string SceneForSlot(RoomKind kind)
    {
        switch (kind)
        {
            case RoomKind.Combat: return PickRandomCombat();
            case RoomKind.Shop:   return shopScene;
            case RoomKind.Boss:   return bossScene;
            default:              return lobbyScene;
        }
    }

    private string PickRandomCombat()
    {
        if (combatPool == null || combatPool.Count == 0)
        {
            Debug.LogError("[RunManager] El 'Combat Pool' está vacío.");
            return lobbyScene;
        }

        if (combatPool.Count == 1) return combatPool[0];

        string chosen;
        int guard = 0;
        do
        {
            chosen = combatPool[Random.Range(0, combatPool.Count)];
            guard++;
        }
        while (chosen == lastCombatScene && guard < 20);

        lastCombatScene = chosen;
        return chosen;
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[RunManager] Nombre de escena vacío.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("[RunManager] La escena '" + sceneName + "' no está en el Build Profile o mal escrita.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}