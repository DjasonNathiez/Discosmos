using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Toolbox.Variable;
using UnityEngine;

public class LevelManager : MonoBehaviourPunCallbacks
{
    private int localPlayerOrder;
    
    public Transform hubSpawnPoint;
    public Transform spawnPointGreenA;
    public Transform spawnPointGreenB;
    public Transform spawnPointPinkA;
    public Transform spawnPointPinkB;
    
    public bool gameStarted;

    private bool allPlayersInRoom;
    private float waitingTimer;
    private float timerBackup;
    public float timeToStart;
    
    public GameObject championSelectCanvas;
    public TextMeshProUGUI waitingForPlayerText;
    public TextMeshProUGUI timer;

    public int countToLoad;
    
    void Start()
    {
        GameAdministrator.instance.localPlayer.PlayerController.transform.position = hubSpawnPoint.position;
        CheckForCount();
    }

    private void Update()
    {
        
        if (PhotonNetwork.CurrentRoom.Players.Count >= countToLoad)
        {
            waitingForPlayerText.text = "Champion select will start...";
            allPlayersInRoom = true;
        }
        else
        {
            waitingForPlayerText.text = PhotonNetwork.CurrentRoom.Players.Count + " / 4";
        }
    }

    public void SetCharacterButton(int characterID)
    {
        GameAdministrator.instance.localPlayer.SendPlayerCharacter(characterID);
    }

    public void SetTeamButton(int teamID)
    {
        GameAdministrator.instance.localPlayer.SendPlayerTeam(teamID);
    }
    

    private void StartGame()
    {
        gameStarted = true;
        championSelectCanvas.SetActive(false);

        switch (GameAdministrator.instance.localPlayer.currentTeam)
        {
            case Enums.Teams.Green:
                GameAdministrator.instance.localPlayer.PlayerController.transform.position = spawnPointGreenA.position;
                break;
            
            case Enums.Teams.Pink:
                GameAdministrator.instance.localPlayer.PlayerController.transform.position = spawnPointPinkA.position;
                break;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        CheckForCount();
    }

    void CheckForCount()
    {
        if (PhotonNetwork.CurrentRoom.Players.Count == countToLoad)
        {
            championSelectCanvas.SetActive(true);
            waitingForPlayerText.gameObject.SetActive(false);
            GameAdministrator.OnServerUpdate += ChampionSelectTimer;
            timerBackup = (float)PhotonNetwork.Time;
            
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    public void ChampionSelectTimer()
    {
        timer.text = Mathf.FloorToInt(timeToStart - waitingTimer).ToString();
        
        if (waitingTimer >= timeToStart)
        {
            StartGame();
            GameAdministrator.OnServerUpdate -= ChampionSelectTimer;
        }
        else
        {
            waitingTimer = (float)(PhotonNetwork.Time - timerBackup);
        }
    }
}
