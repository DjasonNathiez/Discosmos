using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Toolbox.Variable;
using Unity.Mathematics;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
   public static NetworkManager instance;
   [Header("TEST")] 
   public string sceneTestName;
   public ChampionDataSO testChamp;

   [SerializeField] private DebugNetworkShower _debugNetworkShower;

   public GameObject player;
   
   [Header("Connection")]
   [HideInInspector] public LoginScreen LoginScreen;
   private string _playFabPlayerIdCache;
   private bool isSwitchingRoom;

   [Header("Room List")] 
   public List<CustomRoom> roomsList;
   public CustomRoom currentPlayerRoom;
   private string currentTargetingRoom;

   private string roomBackup = "Hub";
   
   RoomOptions roomOptions = new RoomOptions
   {
      IsOpen = true,
      IsVisible = true
   };

   private RaiseEventOptions raiseOption = new RaiseEventOptions
   {
      Receivers = ReceiverGroup.All,
   };
   
   public static NetworkDelegate.OnRoomUpdated OnRoomUpdated;
   
   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   #region Photon Callbacks
   public override void OnConnectedToMaster()
   {
      base.OnConnectedToMaster();
      _debugNetworkShower.photonStatue = "Connected";

      PhotonNetwork.JoinLobby(typedLobby: new TypedLobby("World", LobbyType.Default));
   }

   public override void OnJoinedLobby()
   {
      base.OnJoinedLobby();
      
      _debugNetworkShower.currentLobby = PhotonNetwork.CurrentLobby.Name;
      
      if (GameAdministrator.instance.currentScene == Enums.Scenes.Login)
      {
         GameAdministrator.instance.LoadScene(GameAdministrator.instance.hubSceneName);
      }

      if (!PhotonNetwork.InRoom && !isSwitchingRoom)
      {
         SwitchRoom("Hub");
      }
      else
      {
         JoinRoom();
      }
      
   }

   public void JoinRoom()
   {
      if (GameAdministrator.instance.currentScene == Enums.Scenes.Hub && roomBackup != String.Empty)
      {
         PhotonNetwork.JoinOrCreateRoom(roomBackup, roomOptions, TypedLobby.Default);
      }

      isSwitchingRoom = false;
   }

   public override void OnCreatedRoom()
   {
      base.OnCreatedRoom();
      
      _debugNetworkShower.currentLobby = PhotonNetwork.CurrentLobby.Name;
      _debugNetworkShower.currentRoom = PhotonNetwork.CurrentRoom.Name;
   }

   public override void OnJoinedRoom()
   {
      _debugNetworkShower.currentLobby = PhotonNetwork.CurrentLobby.Name;
      _debugNetworkShower.currentRoom = PhotonNetwork.CurrentRoom.Name;

      if (roomBackup == "Test")
      {
         GameObject playerTest = PhotonNetwork.Instantiate(player.name, Vector3.zero, quaternion.identity);
         
         playerTest.GetPhotonView().Controller.NickName = GameAdministrator.instance.username;
         playerTest.GetComponent<PlayerManager>().championDataSo = testChamp;
         playerTest.GetComponent<PlayerManager>().Initialize();
         
         PhotonNetwork.LoadLevel(sceneTestName);
      }
   }

   public override void OnLeftRoom()
   {
      base.OnLeftRoom();
      _debugNetworkShower.currentLobby = PhotonNetwork.CurrentLobby.Name;
      _debugNetworkShower.currentRoom = "None";

      PhotonNetwork.JoinLobby(typedLobby: new TypedLobby("World", LobbyType.Default));
      
   }

   public override void OnJoinRoomFailed(short returnCode, string message)
   {
      Debug.LogError("Room failed : " + message);
   }

   #endregion

   #region PlayFab Methods
   
   public void RegisterUser(string username, string password)
   {
      var request = new RegisterPlayFabUserRequest
      {
         Username = username,
         Password = password,
         RequireBothUsernameAndEmail = false
      };
      
      PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailed);
   }
   public void ConnectToUser(string username, string password)
   {
      _debugNetworkShower.playFabStatue = "Connecting...";
      var requestPlayFab = new LoginWithPlayFabRequest
      {
         Username = username,
         Password = password,
      };
      
      if(!PlayFabClientAPI.IsClientLoggedIn())
      {
         PlayFabClientAPI.LoginWithPlayFab(requestPlayFab, RequestPhotonToken, OnLoginFailure);
      }
   }
   private void RequestPhotonToken(LoginResult obj)
   {
      _debugNetworkShower.playFabStatue = "Request Photon Token...";
      _playFabPlayerIdCache = obj.PlayFabId;

      PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
      {
         PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
      }, AuthenticateWithPhoton, OnLoginFailure);

   }
   private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {
      var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
      customAuth.AddAuthParameter("username", _playFabPlayerIdCache);   
      customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);
      PhotonNetwork.AuthValues = customAuth;
      
      OnLoginSuccess();
   }
   

   #endregion
   
   #region PlayFab Callbacks

   private void OnLoginSuccess()
   {
      GetPlayerData();
      
      PhotonNetwork.ConnectUsingSettings();
      _debugNetworkShower.photonStatue = "Connecting...";
      _debugNetworkShower.playFabStatue = "Connected";
   }

   private void OnLoginFailure(PlayFabError error)
   {
      Debug.LogError("Something went wrong with login. " + error.GenerateErrorReport());

   }

   private void OnRegisterSuccess(RegisterPlayFabUserResult result)
   {
      SetPlayerData(result.Username);
      
      _debugNetworkShower.photonStatue = "Connecting...";
      _debugNetworkShower.playFabStatue = "Connected";
      PhotonNetwork.ConnectUsingSettings();
   }
   
   private void OnRegisterFailed(PlayFabError error)
   {
      Debug.LogError("Something went wrong with register. " + error.GenerateErrorReport());
   }

   #endregion

   #region Player Data Settings

   public void SetPlayerData(string username)
   {
      var request = new UpdateUserDataRequest
      {
         Data = new Dictionary<string, string>
         {
            {"PlayerLevel", 1.ToString()},
            {"Username", username}
         }
      };

      PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDateSendFailed);
      
      GetPlayerData();
   }

   public void GetPlayerData()
   {
      PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceivedSuccess, OnDataReceivedFailed );
   }

   private void OnDataReceivedFailed(PlayFabError result)
   {
    
   }

   private void OnDataReceivedSuccess(GetUserDataResult result)
   {
      Debug.Log("Received data user");
      if (result != null && result.Data.ContainsKey("PlayerLevel"))
      {
         GameAdministrator.instance.playerLevel = int.Parse(result.Data["PlayerLevel"].Value);
         GameAdministrator.instance.username = result.Data["Username"].Value;
      }
   }

   void OnDataSendSuccess(UpdateUserDataResult result)
   {
      Debug.Log("Successful send user data.");
   }

   void OnDateSendFailed(PlayFabError error)
   {
      
   }

   #endregion
   
   #region Room Management

   public void CreateRoom(string roomName, string roomPassword, Enums.RoomPrivacy roomPrivacy)
   {
      
      foreach (var room in roomsList)
      {
         if (room.roomName == roomName)
         {
            Debug.Log("This room name already exist.");
            return;
         }
      }

      
      CustomRoom newRoom = new CustomRoom
      {
         roomName =  roomName,
         roomPassword = roomPassword,
         roomPrivacy = roomPrivacy,
         roomSize = 4,
         
         owner = new CustomPlayer()
         {
            username = GameAdministrator.instance.username,
            photonViewID = GameAdministrator.instance.localViewID
         },
         
         players = new List<CustomPlayer>
         {
            new()
            {
               username = GameAdministrator.instance.username,
               photonViewID = GameAdministrator.instance.localViewID
            }
         }
         
      };
      
      currentPlayerRoom = newRoom;
      
      //PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
      
      Hashtable options = new Hashtable
      {
         {"RoomName", currentPlayerRoom.roomName},
         {"RoomPassword", currentPlayerRoom.roomPassword},
         {"RoomPrivacy", currentPlayerRoom.roomPrivacy.ToString()}
      };

      PhotonNetwork.RaiseEvent(1, options, raiseOption, SendOptions.SendReliable);
      
      SwitchRoom(roomName);
   }

   public void JoinRoom(string roomName, string roomPassword)
   {
      foreach (var room in roomsList)
      {
         if (room.roomName == roomName)
         {
            if (room.roomPrivacy == Enums.RoomPrivacy.Close)
            {
               if (roomPassword != room.roomPassword)
               {
                  Debug.LogError("Wrong room password");
                  return;
               }
            }
            
            if (room.players.Count >= room.roomSize)
            {
               Debug.LogError(roomName + " is full.");
               return;
            }
            else
            {
               room.players.Add(new CustomPlayer
                  {
                     username = GameAdministrator.instance.username,
                     photonViewID = GameAdministrator.instance.localViewID
                  }
               );
            
               currentPlayerRoom = room;

               //PhotonNetwork.JoinRoom(roomName);
            }
         }
      }
      
      Hashtable options = new Hashtable
      {
         {"RoomName", currentTargetingRoom},
         {"Username", GameAdministrator.instance.username},
         {"PhotonID", GameAdministrator.instance.localViewID}
      };

      PhotonNetwork.RaiseEvent(2, options, raiseOption, SendOptions.SendReliable);
      
      SwitchRoom(roomName);
   }
   
   public void AddNewRoomToList(string roomName, string roomPassword, string roomPrivacy)
   {
      CustomRoom newRoom = new CustomRoom
      {
         roomName =  roomName,
         roomPassword = roomPassword,
         roomPrivacy = Enum.Parse<Enums.RoomPrivacy>(roomPrivacy),
         roomSize = 4,
         
         owner = new CustomPlayer()
         {
            username = GameAdministrator.instance.username,
            photonViewID = GameAdministrator.instance.localViewID
         },
         
         players = new List<CustomPlayer>
         {
            new()
            {
               username = GameAdministrator.instance.username,
               photonViewID = GameAdministrator.instance.localViewID
            }
         }
         
      };
      
      roomsList.Add(newRoom);
      
      Debug.Log("Room List Add");
      
      OnRoomUpdated?.Invoke();
   }
   public void UpdateRoomList(string roomName, string playerUsername, int playerPhotonViewID)
   {
      var player = new CustomPlayer
      {
         username = playerUsername,
         photonViewID = playerPhotonViewID
      };
      
      foreach (var room in roomsList)
      {
         if (room.roomName == roomName)
         {
            if(room.players.Contains(player)) return;
            
            room.players.Add(player);
         }
      }
      
      Debug.Log("Room List Updated");
      
      OnRoomUpdated?.Invoke();
   }

   #endregion

   public void OnEvent(EventData photonEvent)
   {
      if (photonEvent.Code == 200)
      {
         Hashtable options = (Hashtable)photonEvent.CustomData;

         AddNewRoomToList((string)options["RoomName"], (string)options["RoomPassword"], (string)options["RoomPrivacy"]);
      }

      if (photonEvent.Code == 201)
      {
         Hashtable options = (Hashtable)photonEvent.CustomData;

         UpdateRoomList((string)options["RoomName"], (string)options["Username"], (int)options["PhotonID"]);
      }
   }

   public void SwitchRoom(string roomName)
   {
      roomBackup = roomName;
      isSwitchingRoom = true;
      
      if (PhotonNetwork.InRoom)
      {
         PhotonNetwork.LeaveRoom();
      }
      else
      {
         if (PhotonNetwork.InLobby)
         {
            JoinRoom();
         }
         else
         {
            PhotonNetwork.JoinLobby(typedLobby: new TypedLobby("World", LobbyType.Default));
         }
      }
      
   }
}

[Serializable] public class CustomRoom
{
   public string roomName;
   public string roomPassword;
   public int roomSize;
   public Enums.RoomPrivacy roomPrivacy;
   public CustomPlayer owner;
   public List<CustomPlayer> players;
}

[Serializable] public class CustomPlayer
{
   public string username;
   public int photonViewID;
}
