using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Toolbox.Variable;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class HubManager : MonoBehaviour
{
   //Friend List
   
   //Room List

   [Header("Room List")] 
   [SerializeField] private GameObject roomListPrefab;
   [SerializeField] private ScrollRect roomScrollRect;

   
   private List<GameObject> roomListObjects;
   
   [Header("Connect To Room")] 
   [SerializeField] public TextMeshProUGUI joinRoomName;
   [SerializeField] private TMP_InputField joinRoomPassword;
   public GameObject roomConnectPanel;
   
   //Room Creation
   [Header("Creation Room")] 
   [SerializeField] private TMP_InputField createRoomName;
   [SerializeField] private TMP_InputField createRoomPassword;
   [SerializeField] private TMP_Dropdown createRoomPrivacy;
   [SerializeField] private Enums.RoomPrivacy roomPrivacy;

   private void Awake()
   {
      PhotonNetwork.JoinRoom("Hub");
      
      SetRoomPrivacy();
      InitializeRoomList();

      NetworkManager.OnRoomUpdated += UpdateRoomList;
   }

   public void CreateRoomButton()
   {
      if (createRoomName.text.Length > 16)
      {
         Debug.LogError("Room name is too long. 16 characters maximum");
         return;
      }

      if (roomPrivacy != Enums.RoomPrivacy.Open && createRoomPassword.text.Length < 4)
      {
         Debug.LogError("Room passwword is too short. 4 characters minimum");
         return;
      }
      
      NetworkManager.instance.CreateRoom(createRoomName.text, createRoomPassword.text, roomPrivacy);
   }

   public void JoinRoomButton()
   {
      NetworkManager.instance.JoinRoom(joinRoomName.text, joinRoomPassword.text);
   }

   public void SetRoomPrivacy()
   {
      if (createRoomPrivacy.value == 0)
      {
         roomPrivacy = Enums.RoomPrivacy.Open;
         createRoomPassword.gameObject.SetActive(false);
      }

      if (createRoomPrivacy.value == 1)
      {
         roomPrivacy = Enums.RoomPrivacy.Close;
         createRoomPassword.gameObject.SetActive(true);
      }
   }

   private void InitializeRoomList()
   {
      roomListObjects = new List<GameObject>();

      for (int i = 0; i < 20; i++)
      {
         var newRoomListObj = Instantiate(roomListPrefab, roomScrollRect.content);
         roomListObjects.Add(newRoomListObj);
      }

      UpdateRoomList();
   }

   public void UpdateRoomList()
   {
      foreach (var roomObj in roomListObjects)
      {
         roomObj.SetActive(false);
      }
      
      for (int i = 0; i < NetworkManager.instance.roomsList.Count; i++)
      {
         roomListObjects[i].SetActive(true);
         var roomUiItem = roomListObjects[i].GetComponent<RoomUIItem>();
         var room = NetworkManager.instance.roomsList[i];

         roomUiItem.roomNameTxt.text = room.roomName;
         roomUiItem.roomOwnerTxt.text = room.owner.username;
         roomUiItem.roomPrivacyTxt.text = room.roomPrivacy.ToString();
         roomUiItem.roomSizeTxt.text = room.players.Count + "/" + room.roomSize;
      }
   }
   
   //Room Inside
}
