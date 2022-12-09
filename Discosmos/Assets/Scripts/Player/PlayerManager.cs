using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Toolbox.Variable;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] public string username;
    public PlayerController PlayerController;
    public GameObject hud;
    public GameObject cam;
    public IPlayer iplayer;
    public ChampionDataSO championDataSo;

    [Header("Stats and UI")]
    
    [SerializeField] private bool overwriteOnline;
    
    [SerializeField] private Image healthBar;
    [SerializeField] public Image speedBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform uiStatsTransform;
    [SerializeField] private Transform target;
    [SerializeField] private float heightUI;
    [SerializeField] public Camera _camera;
    [SerializeField] private GameObject healthBarObj;
    [SerializeField] private Transform canvas;
    
    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    public int currentShield;
    public bool isCasting;
    public bool canMove;

    [Header("Movement")]
    public float currentSpeed;
    public float normalSpeed;
    public float groovySpeed;
    public float speedMultiplier;
    
    [Header("FX")]
    public ParticleSystem attackFx;
    public ParticleSystem laserFX;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!overwriteOnline)
        {
            GameAdministrator.instance.localViewID = GetComponent<PhotonView>().ViewID; 
            GameAdministrator.instance.localPlayerView = GetComponent<PhotonView>();   
        }

        GameObject healthBarObject = Instantiate(healthBarObj, Vector3.zero, quaternion.identity, canvas);
        uiStatsTransform = healthBarObject.transform;
        healthBar = uiStatsTransform.GetChild(0).GetComponent<Image>();
        speedBar = uiStatsTransform.GetChild(1).GetComponent<Image>();
        healthText = uiStatsTransform.GetChild(2).GetComponent<TextMeshProUGUI>();
        nameText = uiStatsTransform.GetChild(3).GetComponent<TextMeshProUGUI>();
        target = uiStatsTransform.GetChild(4);
    }

    private void Start()
    {
        username = photonView.Controller.NickName;
        nameText.text = username;
    }

    public void ShowTarget()
    {
        target.gameObject.SetActive(true);
    }
    
    public void HideTarget()
    {
        target.gameObject.SetActive(false);
    }

    public void CallFX(VisualEffects effect)
    {
        switch (effect)
        {
            case VisualEffects.MimiAutoAttack:
                attackFx.Play();
                break;
            case VisualEffects.MimiLaser:
                laserFX.Play();
                break;
        }
    }

    public void Initialize()
    {
        canMove = true;
        
        if (championDataSo)
        {
            PlayerController.ActiveCapacity1SO = championDataSo.ActiveCapacity1So;
            PlayerController.ActiveCapacity2SO = championDataSo.ActiveCapacity2So;
            PlayerController.UltimateCapacitySO = championDataSo.UltimateCapacitySo;
        }
        
        if (photonView.IsMine)
        {
            PlayerController.enabled = true;
            hud.SetActive(true);
            cam.SetActive(true);
            
            GameAdministrator.instance.localPlayer = this;
        }
    }

    private void LateUpdate()
    {
        SetUI();
    }

    void SetUI()
    {
        uiStatsTransform.position = GameAdministrator.instance.localPlayer._camera.WorldToScreenPoint(PlayerController.transform.position + Vector3.up) + Vector3.up * heightUI;
        healthBar.fillAmount = currentHealth / (float) maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    private void OnDestroy()
    {
        Debug.Log("Have been destroy");
    }

    public void DealDamage(int[] targetsID, int damageAmount)
    {
        Hashtable data = new Hashtable
        {
            {"TargetsID", targetsID},
            {"Amount", damageAmount}
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};

        PhotonNetwork.RaiseEvent(RaiseEvent.DamageTarget, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == RaiseEvent.DamageTarget)
        {
            Debug.Log("Damage event reiceveid");
            Hashtable data = (Hashtable)photonEvent.CustomData;
            int[] targets = (int[])data["TargetsID"];

            foreach (int id in targets)
            {
                if (photonView.ViewID == id)
                {
                    TakeDamage(data);
                }
            }
        }

        if (photonEvent.Code == RaiseEvent.Death)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            
            if (photonView.ViewID == (int)data["ID"])
            {
                Death();
            }
        }
    }

    public void TakeDamage(Hashtable data)
    {
        int amount = (int)data["Amount"];
        
        if (currentShield > 0)
        {
            int holdingDamage = amount - currentShield;

            currentShield -= amount;

            if (holdingDamage > 0)
            {
                currentHealth -= amount;
            }
        }
        else
        {
            currentHealth -= amount;
        }
        
        Debug.Log("Take Damage");
        
        if(currentHealth <= 0)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, };
            PhotonNetwork.RaiseEvent(RaiseEvent.Death, new Hashtable{{"ID", photonView.ViewID}}, raiseEventOptions, SendOptions.SendReliable);
        }
        
    }

    void Death()
    {
        //TODO ALL LOGIC BRO WTF
        Respawn();
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        PlayerController.transform.position = FindObjectOfType<LevelManager>().spawnPoint.position;
        PlayerController.agent.ResetPath();
    }
}

public enum VisualEffects
{
    MimiAutoAttack,
    MimiLaser
}

public enum Capacities
{
    MIMI_Laser
}
