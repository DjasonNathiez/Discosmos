using System;
using Photon.Pun;
using Toolbox.Variable;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
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
    public string username;
    public int playerLevel;

    [Header("Local Player In Game Informations")]
    public Enums.Teams currentTeam;

    [Header("Local Player")] 
    public static string username;

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

    public void LoadScene(Enums.Scenes scenes)
    {
        SceneManager.LoadScene(GetSceneName(scenes));
        currentScene = scenes;
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
}
