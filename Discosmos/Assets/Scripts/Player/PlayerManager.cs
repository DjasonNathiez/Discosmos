using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPlayer
{
    public ChampionDataSO championDataSo;

    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    public int currentShield;
    
    [Header("Movement")]
    public float currentSpeed;
    public float normalSpeed;
    public float groovySpeed;
    public float speedMultiplier;

    private void Awake()
    {
        Debug.Log("Instantiate");
        GameAdministrator.instance.localViewID = GetComponent<PhotonView>().ViewID; 
        GameAdministrator.instance.localPlayerView = GetComponent<PhotonView>();
    }

    private void OnDestroy()
    {
        Debug.Log("Have been destroy");
    }
}
