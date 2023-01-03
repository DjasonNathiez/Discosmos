using System;
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
        NetworkManager.instance.LoginScreen = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConnectButton();
        }
    }

    public void SetPlayerAccount(int id)
    {
        switch (id)
        {
            case 0:
                usernameInputField.text = "admin";
                passwordInputField.text = "ZHubaxWIXLZn6oYm";
                break;
            
            case 1:
                usernameInputField.text = "disco";
                passwordInputField.text = "mimivega";
                break;
            
            case 2:
                usernameInputField.text = "mimi";
                passwordInputField.text = "lasershot";
                break;
            
            case 3:
                usernameInputField.text = "vega";
                passwordInputField.text = "blackhole";
                break;
        }
    }
    
    public void ActiveConnectPanel(bool state)
    {
        connectPanel.SetActive(state);
    }
    
    public void RegisterButton()
    {
        NetworkManager.instance.RegisterUser(usernameInputField.text, passwordInputField.text);
    }

    public void ConnectButton()
    {
        NetworkManager.instance.ConnectToUser(usernameInputField.text, passwordInputField.text);
    }
}
