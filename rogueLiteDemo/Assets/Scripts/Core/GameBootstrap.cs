using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [Header("Boot Settings")]
    [SerializeField] private string firstScene = "MainMenu";
    [Tooltip("Segundos que se muestra la pantalla RECUERDA antes de ir al menú.")]
    [SerializeField] private float titleDuration = 3f;

    [Header("Core Prefabs (optional)")]
    [SerializeField] private GameManager gameManagerPrefab;

    void Awake()
    {
        InitializeSystems();
    }

    void Start()
    {
        StartCoroutine(ShowTitleThenLoad());
    }

    private IEnumerator ShowTitleThenLoad()
    {
        yield return new WaitForSeconds(titleDuration);
        LoadFirstScene();
    }

    private void InitializeSystems()
    {
        EnsureGameManager();
        EnsureWallet();
        Debug.Log("[Bootstrap] Core systems initialized.");
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance != null) return;

        if (gameManagerPrefab != null) Instantiate(gameManagerPrefab);
        else
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.LogWarning("[Bootstrap] GameManager prefab not assigned — created at runtime.");
        }
    }

    private void EnsureWallet()
    {
        if (PlayerWallet.instance != null) return;
        GameObject go = new GameObject("PlayerWallet");
        go.AddComponent<PlayerWallet>();
        DontDestroyOnLoad(go);
        Debug.Log("[Bootstrap] PlayerWallet persistente creada.");
    }

    private void LoadFirstScene()
    {
        if (string.IsNullOrEmpty(firstScene))
        {
            Debug.LogError("[Bootstrap] firstScene name is empty.");
            return;
        }
        SceneManager.LoadScene(firstScene);
    }
}