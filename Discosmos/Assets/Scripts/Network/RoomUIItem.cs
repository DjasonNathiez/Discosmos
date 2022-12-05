using TMPro;
using UnityEngine;

public class RoomUIItem : MonoBehaviour
{
    public TextMeshProUGUI roomNameTxt;
    public TextMeshProUGUI roomOwnerTxt;
    public TextMeshProUGUI roomPrivacyTxt;
    public TextMeshProUGUI roomSizeTxt;

    public void TryJoinRoom()
    {
        HubManager hubManager = FindObjectOfType<HubManager>();

        hubManager.joinRoomName.text = roomNameTxt.text;
        hubManager.roomConnectPanel.SetActive(true);
        
    }
}
