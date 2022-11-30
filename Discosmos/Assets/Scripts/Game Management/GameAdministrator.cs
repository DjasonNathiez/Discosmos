using Photon.Pun;
using Toolbox.Variable;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameAdministrator : MonoBehaviour
{
    [Header("Network")] 
    public double tickRate;
    
    public static NetworkDelegate.OnServerUpdate OnServerUpdate;
    private NetworkDelegate.OnUpdated OnUpdated;
    
    private double timer;
    private double lastTickTime;

    [Header("Game State")] 
    public Enums.GameState currentState;

    private void Awake()
    {
        OnUpdated += UpdateNetwork;
    }

    void Update()
    {
        switch (currentState)
        {
            case Enums.GameState.InGame:
                OnUpdated?.Invoke();
                break;
        }
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
}
