using System;
using Photon.Pun;
using Toolbox.Variable;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAdministrator : MonoBehaviour
{
    public static GameAdministrator instance;

    [Header("Network")] 
    public double tickRate;
    public static NetworkDelegate.OnServerUpdate OnServerUpdate;
    private NetworkDelegate.OnUpdated OnUpdated;
    public static NetworkDelegate.OnCapacityPerform OnCapacityPerform;
    private double timer;
    private double lastTickTime;

    [Header("Game State")] 
    public Enums.GameState currentState;
    public Enums.Scenes currentScene;

    [Space] 
    public string loginSceneName;
    public string hubSceneName;
    public string gameSceneName;
    public string endGameSceneName;

    [Header("Local Player")] 
    public PlayerManager localPlayer;
    public bool localInitialize;
    public string username;
    public int playerLevel;
    public int localViewID;
    public PhotonView localPlayerView;

    [Header("Local Player In Game Informations")]
    public Enums.Teams currentTeam;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        #endregion
        
        OnUpdated += UpdateNetwork;

        string activeSceneName = SceneManager.GetActiveScene().name;
        
        if (activeSceneName == loginSceneName)
        {
            currentScene = Enums.Scenes.Login;
        }
        else if (activeSceneName == hubSceneName)
        {
            currentScene = Enums.Scenes.Hub;
        }
        else if (activeSceneName == gameSceneName)
        {
            currentScene = Enums.Scenes.Game;
        }
        else if (activeSceneName == endGameSceneName)
        {
            currentScene = Enums.Scenes.EndGame;
        }
    }

    void Update()
    {
        OnUpdated?.Invoke();
    }

    void UpdateNetwork()
    {
        if (timer >= 1.00 / tickRate)
        {
            Tick();
            lastTickTime = PhotonNetwork.Time;
        }
        else
        {
            timer = PhotonNetwork.Time - lastTickTime;
        }
    }

    void Tick()
    {
        OnServerUpdate?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        if (currentScene == GetSceneByName(sceneName)) return;
        SceneManager.LoadScene(sceneName);
        currentScene = GetSceneByName(sceneName);
        SetGameState();
    }

    public void SetGameState()
    {
        switch (currentScene)
        {
            default:
                currentState = Enums.GameState.Loading;
                break;
            
            case Enums.Scenes.Hub:
                currentState = Enums.GameState.Hub;
                break;
            case Enums.Scenes.Login:
                currentState = Enums.GameState.Login;
                break;
            case Enums.Scenes.Game:
                currentState = Enums.GameState.InGame;
                break;
        }
    }

    public string GetSceneName(Enums.Scenes scenes)
    {
        switch (scenes)
        {
            case Enums.Scenes.Login:
                return loginSceneName;
            
            case Enums.Scenes.Hub:
                return hubSceneName;
            
            case Enums.Scenes.Game:
                return gameSceneName;
            
            case Enums.Scenes.EndGame:
                return endGameSceneName;
        }
        
        return String.Empty;
    }

    public Enums.Scenes GetSceneByName(string sceneName)
    {
        switch (sceneName)
        {
            default:
                return Enums.Scenes.Login;
            
            case "LoginScene":
                return Enums.Scenes.Login;
            
            case "HubScene" :
                return Enums.Scenes.Hub;

            case "Test":
                return Enums.Scenes.Game;
        }
    }
}
