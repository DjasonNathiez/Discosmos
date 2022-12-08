using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks, IPlayer
{
    [SerializeField] 
    public PlayerController PlayerController;
    
    public ChampionDataSO championDataSo;

    [Header("Stats and UI")] 
    
    [SerializeField] private bool overwriteOnline;
    
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Transform uiStatsTransform;
    [SerializeField] private float heightUI;
    [SerializeField] public Camera _camera;
    [SerializeField] private GameObject healthBarObj;
    
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

        Transform canvas = FindObjectOfType<Canvas>().transform;
        GameObject healthBarObject = Instantiate(healthBarObj, Vector3.zero, quaternion.identity,canvas);
        uiStatsTransform = healthBarObject.transform;
        healthBar = uiStatsTransform.GetChild(0).GetComponent<Image>();
        healthText = uiStatsTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void Initialize()
    {
        if (photonView.IsMine)
        {
            PlayerController.enabled = true;
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
        uiStatsTransform.position = _camera.WorldToScreenPoint(transform.position + Vector3.up) + Vector3.up * heightUI;
        healthBar.fillAmount = currentHealth / (float) maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    private void OnDestroy()
    {
        Debug.Log("Have been destroy");
    }
}
