using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby_3D");
    }

    public void LoadRoomTest01()
    {
        SceneManager.LoadScene("Room_Test_01");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}