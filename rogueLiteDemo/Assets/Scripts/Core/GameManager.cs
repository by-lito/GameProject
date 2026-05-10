using System;
using UnityEngine;

/// <summary>
/// Central game state manager. Singleton, persists across scenes.
/// Handles: Menu, Playing, Paused, Dead states.
/// Other systems subscribe to OnStateChanged to react to transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Paused, Dead }

    public GameState CurrentState { get; private set; } = GameState.Menu;

    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        // Singleton — only one GameManager across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── State transitions ────────────────────────────────────────────

    public void StartRun()
    {
        SetState(GameState.Playing);
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused) return;
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void EndRun(bool playerDied)
    {
        Time.timeScale = 1f;
        SetState(playerDied ? GameState.Dead : GameState.Menu);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SetState(GameState.Menu);
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] State → {newState}");
    }
}