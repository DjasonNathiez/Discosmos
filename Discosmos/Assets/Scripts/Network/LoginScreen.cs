using TMPro;
using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    [Header("Connection")] 
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    private void Start()
    {
        PlayFabManager.instance.LoginScreen = this;
    }

    public void ActiveConnectPanel(bool state)
    {
        connectPanel.SetActive(state);
    }
    
    public void RegisterButton()
    {
        PlayFabManager.instance.RegisterUser(usernameInputField.text, passwordInputField.text);
    }

    public void ConnectButton()
    {
        PlayFabManager.instance.ConnectToUser(usernameInputField.text, passwordInputField.text);
    }
}
