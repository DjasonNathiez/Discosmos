using System;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabLogin : MonoBehaviourPunCallbacks
{
   [Header("Connection")] 
   [SerializeField] private GameObject connectPanel;
   [SerializeField] private TMP_InputField usernameInputField;
   [SerializeField] private TMP_InputField passwordInputField;
   [SerializeField] private TextMeshProUGUI popUpTextMeshProUGUI;

   [SerializeField] private float popUpDuration;
   private float fadeEffectTimer;
   private float fadePopUpTimer;
   private double serverTimeBackup;

   private void Awake()
   {
      PhotonNetwork.ConnectUsingSettings();
   }

   #region Photon Callbacks

   public override void OnConnectedToMaster()
   {
      base.OnConnectedToMaster();
      connectPanel.SetActive(true);
      
      SetPopUpMessage("Connected to server", Color.green);
   }


   #endregion

   #region PlayFab Methods
   
   public void RegisterUser()
   {
      var request = new RegisterPlayFabUserRequest
      {
         Username = usernameInputField.text,
         Password = passwordInputField.text,
         RequireBothUsernameAndEmail = false
      };
      
      PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailed);
   }
   public void ConnectToUser()
   {
      var requestPlayFab = new LoginWithPlayFabRequest
      {
         Username = usernameInputField.text,
         Password = passwordInputField.text,
      };
      
      if(PlayFabClientAPI.IsClientLoggedIn())
      {
         SetPopUpMessage("You're already connected", Color.white);
      }
      else
      {
         PlayFabClientAPI.LoginWithPlayFab(requestPlayFab, OnLoginSuccess, OnLoginFailure);
      }
   }
   #endregion
   
   #region PlayFab Callbacks

   private void OnLoginSuccess(LoginResult result)
   {
      SetPopUpMessage("You've been connected", Color.white);
      GameAdministrator.username = usernameInputField.text;
      connectPanel.SetActive(false);
   }
   private void OnLoginFailure(PlayFabError error)
   {
      SetPopUpMessage(error.GenerateErrorReport(), Color.red);
   }

   private void OnRegisterSuccess(RegisterPlayFabUserResult result)
   {
      SetPopUpMessage("You've been registered", Color.white);
   }
   private void OnRegisterFailed(PlayFabError error)
   {
      SetPopUpMessage(error.GenerateErrorReport(), Color.red);
   }

   #endregion
   
   #region Visual Feedback

   public void SetPopUpMessage(string message, Color color = new Color())
   {
      popUpTextMeshProUGUI.text = message;
      popUpTextMeshProUGUI.color = color;
      serverTimeBackup = PhotonNetwork.Time;
      GameAdministrator.OnServerUpdate += FadePopUp;
   }
   private void FadePopUp()
   {
      //One time activation
      bool isEnable = false;
      if (!isEnable)
      {
         popUpTextMeshProUGUI.gameObject.SetActive(true);
         isEnable = true;
      }

      if (fadePopUpTimer > popUpDuration)
      {
         popUpTextMeshProUGUI.gameObject.SetActive(false);
         GameAdministrator.OnServerUpdate -= FadePopUp;
         fadePopUpTimer = 0;
      }
      else
      {
         fadePopUpTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
         fadeEffectTimer = popUpDuration - fadePopUpTimer;

         var color = popUpTextMeshProUGUI.color;
         color.a = fadeEffectTimer;
      
         popUpTextMeshProUGUI.color = color;
      }
   }

   #endregion

}
