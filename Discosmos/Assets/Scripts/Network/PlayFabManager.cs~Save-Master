using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Toolbox.Variable;
using UnityEngine;

public class PlayFabManager : MonoBehaviourPunCallbacks
{
   public static PlayFabManager instance;

   [Header("Connection")]
   [HideInInspector] public LoginScreen LoginScreen;

   private string _playFabPlayerIdCache;

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

   private void Start()
   {
      PhotonNetwork.ConnectUsingSettings();
   }

   #region Photon Callbacks
   public override void OnConnectedToMaster()
   {
      base.OnConnectedToMaster();
      if (GameAdministrator.instance.currentState == Enums.GameState.Login)
      {
         LoginScreen.ActiveConnectPanel(true);
      }
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
      
      GameAdministrator.instance.LoadScene(Enums.Scenes.Hub);
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
   

}
