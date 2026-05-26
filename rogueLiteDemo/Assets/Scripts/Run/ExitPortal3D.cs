using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal3D : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = SceneLoader.Lobby3D;

    private bool isLoading;

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;

        if (!other.CompareTag("Player")) return;

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("[ExitPortal3D] El portal no tiene escena destino configurada.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError("[ExitPortal3D] La escena '" + sceneToLoad + "' no está en el Build Profile o está mal escrita.", this);
            return;
        }

        isLoading = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}