using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DebugNetworkShower : MonoBehaviour
{
    public bool debugIsActive;
    [Header("Showers")]
    public bool connectDebug;
    public bool networkStatueDebug;

    public GameObject connectDebugObj;
    public TextMeshProUGUI currentNetworkStatue;

    public string photonStatue = "Disconnected";
    public string playFabStatue = "Disconnected";
    public string currentLobby = "None";
    public string currentRoom = "None";
    public string ping = "0";

    private void Update()
    {
        if (networkStatueDebug)
        {
            ping = PhotonNetwork.GetPing().ToString();
            SetNetworkStatueDebug();
        }
    }

    public void SetNetworkStatueDebug()
    {
        currentNetworkStatue.text = String.Format(
            "Photon : {0}, PlayFabAuth : {1}, Lobby : {2}, Room : {3}, Ping : {4}"
            , photonStatue, playFabStatue, currentLobby, currentRoom, ping);
    }
    
    public void ConnectDebug()
    {
        NetworkManager.instance.ConnectToUser("Edaryl", "WindsofYRUIS23");
    }
}
