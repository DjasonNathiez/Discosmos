using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Toolbox.Variable;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
   public static NetworkManager instance;

   [SerializeField] private DebugNetworkShower _debugNetworkShower;

   public GameObject player;
   
   [Header("Connection")]
   [HideInInspector] public LoginScreen LoginScreen;
   private string _playFabPlayerIdCache;

   [Header("Room List")] 
   public List<CustomRoom> roomsList;
   public CustomRoom currentPlayerRoom;
   private string currentTargetingRoom;
   
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
      
      if (GameAdministrator.instance.currentScene == Enums.Scenes.Login)
      {
         LoginScreen.ActiveConnectPanel(true);
      }

      PhotonNetwork.JoinLobby(typedLobby: new TypedLobby("World", LobbyType.Default));
   }

   public override void OnJoinedLobby()
   {
      base.OnJoinedLobby();
      
      if (!GameAdministrator.instance.localInitialize)
      {
         PhotonNetwork.JoinRandomOrCreateRoom();
      }
   }

   public override void OnCreatedRoom()
   {
      base.OnCreatedRoom();
      GameAdministrator.instance.localPlayerView.RPC("AddNewRoomToList", RpcTarget.AllBufferedViaServer, currentPlayerRoom.roomName, currentPlayerRoom.roomPassword, currentPlayerRoom.roomPrivacy.ToString());
   }

   public override void OnJoinedRoom()
   {
      if (GameAdministrator.instance.currentScene == Enums.Scenes.Login)
      {
         GameAdministrator.instance.LoadScene(Enums.Scenes.Hub);
      }
      
      if (GameAdministrator.instance.currentScene == Enums.Scenes.Hub)
      {
         GameAdministrator.instance.localPlayerView.RPC("UpdateRoomList", RpcTarget.AllBufferedViaServer, currentTargetingRoom, GameAdministrator.instance.username, GameAdministrator.instance.localViewID);
      }
      _debugNetworkShower.currentLobby = PhotonNetwork.CurrentLobby.Name;
      _debugNetworkShower.currentRoom = PhotonNetwork.CurrentRoom.Name;
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
      
      GameAdministrator.instance.LoadScene(Enums.Scenes.Hub);
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
      RoomOptions roomOptions = new RoomOptions
      {
         CustomRoomProperties = new Hashtable()
         {
            {"Password", roomPassword}
         },
         MaxPlayers = 4
      };
      
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
      
      PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
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

               PhotonNetwork.JoinRoom(roomName);
            }
         }
      }
   }
   
   [PunRPC] public void AddNewRoomToList(string roomName, string roomPassword, string roomPrivacy)
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
      
      FindObjectOfType<HubManager>().UpdateRoomList();
   }
   [PunRPC] public void UpdateRoomList(string roomName, string playerUsername, int playerPhotonViewID)
   {
      var player = new CustomPlayer
      {
         username = GameAdministrator.instance.username,
         photonViewID = GameAdministrator.instance.localViewID
      };
      
      foreach (var room in roomsList)
      {
         if (room.roomName == roomName)
         {
            if(room.players.Contains(player)) return;
            
            room.players.Add(player);
         }
      }
      
      FindObjectOfType<HubManager>().UpdateRoomList();
      
   }

   #endregion
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
