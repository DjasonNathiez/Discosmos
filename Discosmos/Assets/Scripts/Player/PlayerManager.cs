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
    [SerializeField] 
    public PlayerController PlayerController;
    public GameObject hud;
    public GameObject cam;
    public IPlayer iplayer;
    public ChampionDataSO championDataSo;

    [Header("Stats and UI")]
    
    [SerializeField] private bool overwriteOnline;
    
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform uiStatsTransform;
    [SerializeField] private float heightUI;
    [SerializeField] public Camera _camera;
    [SerializeField] private GameObject healthBarObj;
    [SerializeField] private Transform canvas;
    
    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    public int currentShield;

    [Header("Movement")]
    public float currentSpeed;
    public float normalSpeed;
    public float groovySpeed;
    public float speedMultiplier;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!overwriteOnline)
        {
            GameAdministrator.instance.localViewID = GetComponent<PhotonView>().ViewID; 
            GameAdministrator.instance.localPlayerView = GetComponent<PhotonView>();   
        }
        iplayer = GetComponent<IPlayer>();
        GameObject healthBarObject = Instantiate(healthBarObj, Vector3.zero, quaternion.identity, canvas);
        uiStatsTransform = healthBarObject.transform;
        healthBar = uiStatsTransform.GetChild(0).GetComponent<Image>();
        healthText = uiStatsTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText = uiStatsTransform.GetChild(2).GetComponent<TextMeshProUGUI>();
        nameText.text = GameAdministrator.instance.username;
    }

    public void Initialize()
    {
        if (photonView.IsMine)
        {
            PlayerController.enabled = true;
            hud.SetActive(true);
            cam.SetActive(true);
            
            GameAdministrator.instance.localPlayer = this;
        }
        
        //PLAYER CUSTOM PROPERTIES
        Hashtable customPropertiesBase = new Hashtable
        {
            {"CurrentHealth", currentHealth },
            {"MaxHealth", maxHealth },
            {"CurrentShield", currentShield},
            {"CurrentSpeed", currentSpeed}
        };
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

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, };

        PhotonNetwork.RaiseEvent(PlayerRaiseEvent.DamageTarget, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == PlayerRaiseEvent.DamageTarget)
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
    }
}
