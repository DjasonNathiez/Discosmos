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
    public static NetworkDelegate.OnCapacityPerform OnCapacityPerform;
    private double timer;
    private double lastTickTime;

    [Header("Game State")] 
    public Enums.GameState currentState;

    [Header("Local Player")] 
    public static string username;

    private void Awake()
    {
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
}
