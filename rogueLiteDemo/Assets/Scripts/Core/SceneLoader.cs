using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public const string MainMenu = "MainMenu";
    public const string Lobby3D = "Lobby_3D";
    public const string Prototype3DBase = "Prototype_3D_Base";
    public const string RoomCombat01 = "Room_Combat_01";
    public const string RoomShop01 = "Room_Shop_01";

    public void LoadMainMenu()
    {
        LoadScene(MainMenu);
    }

    public void LoadLobby()
    {
        LoadScene(Lobby3D);
    }

    public void LoadPrototype3DBase()
    {
        LoadScene(Prototype3DBase);
    }

    public void LoadRoomCombat01()
    {
        LoadScene(RoomCombat01);
    }

    public void LoadRoomShop01()
    {
        LoadScene(RoomShop01);
    }

    public void LoadRoomTest01()
    {
        Debug.LogWarning("[SceneLoader] LoadRoomTest01 es antiguo. Redirigiendo a Room_Combat_01.");
        LoadScene(RoomCombat01);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneLoader] No se puede cargar una escena con nombre vacío.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("[SceneLoader] La escena '" + sceneName + "' no está añadida al Build Profile o el nombre está mal escrito.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}