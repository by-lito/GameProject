using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedSceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene = "Lobby_3D";
    [SerializeField] private float duration = 3f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(duration);

        if (!string.IsNullOrWhiteSpace(nextScene) && Application.CanStreamedLevelBeLoaded(nextScene))
            SceneManager.LoadScene(nextScene);
        else
            Debug.LogError("[TimedSceneLoader] Escena destino vacía o no está en el Build: " + nextScene);
    }
}