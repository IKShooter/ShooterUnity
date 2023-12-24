using UnityEngine;

using LiteNetLib;
using UnityEngine.SceneManagement;

public class ClientConnect : MonoBehaviour
{
    private static ClientConnect instance;
    public static ClientConnect Instance => instance;

    public Player LocalPlayer;

    string serverAddres = "77.246.100.110";
    string appKey = "Shooter";
    int serverPort = 9051;

    public static NetManager client;

    public enum StateScene
    {
        Auth,
        MainMenu,
        InRoom
    }

    public StateScene stateScene = StateScene.Auth;

    void Start()
    {
        client = new NetManager(new ClientListener());
        client.Start();
        client.Connect(serverAddres, serverPort, appKey);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void Update()
    {
        if (instance == null) instance = this;

        if(client.IsRunning)
        {
            client.PollEvents();
        }
    }

    private void OnApplicationQuit()
    {
        client.Stop();
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        switch(arg0.name)
        {
            case "Auth":
                stateScene = StateScene.Auth;
                ClientListener.CurrenRoom = null;
                break;
            case "MainMenu":
                stateScene = StateScene.MainMenu;
                ClientListener.CurrenRoom = null;
                break;
            default:
                stateScene = StateScene.InRoom;
                break;
        }
    }
}