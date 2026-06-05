using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal3D : MonoBehaviour
{
    public enum PortalMode { AdvanceRun, StartRun, LoadScene }

    [Header("Comportamiento")]
    [SerializeField] private PortalMode mode = PortalMode.AdvanceRun;

    [Header("Solo para modo LoadScene")]
    [SerializeField] private string sceneToLoad = SceneLoader.Lobby3D;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        if (!other.CompareTag("Player")) return;

        switch (mode)
        {
            case PortalMode.StartRun:
                if (RunManager.Instance == null)
                {
                    Debug.LogError("[ExitPortal3D] No hay RunManager. ¿Arrancaste desde Boot?", this);
                    return;
                }
                isTriggered = true;
                RunManager.Instance.StartRun();
                break;

            case PortalMode.AdvanceRun:
                if (RunManager.Instance == null)
                {
                    Debug.LogError("[ExitPortal3D] No hay RunManager. ¿Arrancaste desde Boot?", this);
                    return;
                }
                isTriggered = true;
                RunManager.Instance.LoadNextRoom();
                break;

            case PortalMode.LoadScene:
                LoadSpecificScene();
                break;
        }
    }

    private void LoadSpecificScene()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("[ExitPortal3D] No hay escena destino configurada.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError("[ExitPortal3D] La escena '" + sceneToLoad + "' no está en el Build Profile o mal escrita.", this);
            return;
        }

        isTriggered = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}