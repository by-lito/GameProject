using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal3D : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Lobby";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}