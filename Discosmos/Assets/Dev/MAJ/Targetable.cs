using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Targetable : MonoBehaviour
{
    [HideInInspector] public HealthBar healthBar;
    public GameObject uiObject;
    public PhotonView masterPhotonView;
    [HideInInspector] public int photonID;
    public Transform targetableBody;
    public float heightUI;
    public UIType type;

    private void Awake()
    {
        if (!masterPhotonView)
        {
            masterPhotonView = PhotonView.Get(gameObject);
        }
        
        photonID = masterPhotonView.ViewID;
    }

    private void Start()
    {
        CreateUI();
    }

    public void CreateUI()
    {
        switch (type)
        {
            case UIType.PlayerUI:
                healthBar.transform = Instantiate(uiObject, Vector3.zero, quaternion.identity, GameAdministrator.instance.localPlayer.canvas).transform;
                healthBar.healthFill = healthBar.transform.GetChild(0).GetComponent<Image>();
                healthBar.speedFill =  healthBar.transform.GetChild(1).GetComponent<Image>();
                healthBar.healthText =  healthBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                healthBar.nameText =  healthBar.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                healthBar.target =  healthBar.transform.GetChild(4);
                if(healthBar.name != string.Empty) healthBar.nameText.text = healthBar.name;
                break;
            
            case UIType.ClassicUI:
                healthBar.transform = Instantiate(uiObject, Vector3.zero, quaternion.identity, GameAdministrator.instance.localPlayer.canvas).transform;
                healthBar.healthFill = healthBar.transform.GetChild(0).GetComponent<Image>();
                healthBar.healthText = healthBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                healthBar.target = healthBar.transform.GetChild(2);
                break;
        }
    }
    public void ShowTarget()
    {
        healthBar.target.gameObject.SetActive(true);
    }
    
    public void HideTarget()
    {
        healthBar.target.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        healthBar.transform.position = GameAdministrator.instance.localPlayer._camera.WorldToScreenPoint(targetableBody.position + Vector3.up) + Vector3.up * heightUI;   
    }

    public void UpdateUI(bool updatePos,bool updateHealth = false,int currentHealth = 0,int maxHealth = 0,bool updateSpeed = false,float speed = 0,bool updateName = false, string name = "[not defined]")
    {
        if (updateHealth)
        {
            healthBar.healthFill.fillAmount = currentHealth / (float) maxHealth;
            healthBar.healthText.text = currentHealth + " / " + maxHealth;   
        }
        if (updateSpeed)
        {
            healthBar.speedFill.fillAmount = speed;
        }
        if (updateName)
        {
            healthBar.name = name;
            if(healthBar.nameText) healthBar.nameText.text = name;
        }
    }
}

public enum UIType
{
    PlayerUI,
    ClassicUI
}

[Serializable]
public class HealthBar
{
    public Transform transform;
    public Image healthFill;
    public Image speedFill;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI nameText;
    public Transform target;
    public string name;
}

