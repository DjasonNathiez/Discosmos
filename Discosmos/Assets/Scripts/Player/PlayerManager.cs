using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPlayer
{
    [SerializeField] 
    public PlayerController PlayerController;
    
    public ChampionDataSO championDataSo;

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
        GameAdministrator.instance.localViewID = GetComponent<PhotonView>().ViewID; 
        GameAdministrator.instance.localPlayerView = GetComponent<PhotonView>();
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

    private void OnDestroy()
    {
        Debug.Log("Have been destroy");
    }
}
