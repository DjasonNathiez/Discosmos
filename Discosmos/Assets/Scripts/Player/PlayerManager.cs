using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Toolbox.Variable;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback, ITeamable
{
    [HideInInspector] public string username;
    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public GameObject hud;
    [HideInInspector] public Transform canvas;
    [HideInInspector] public GameObject cam;
    [HideInInspector] public IPlayer iplayer;
    public Enums.Teams currentTeam;
    public ChampionDataSO championDataSo;
    [HideInInspector] public PlayerAnimationScript playerAnimationScript;
    [HideInInspector] public CameraController cameraController;
    [HideInInspector] public GameObject modelBody;
    public Transform fogOfWarRenderer;
    public GameObject mimiModel;
    public GameObject vegaModel;
    public PlayerAnimationScript mimiAnimScript;
    public PlayerAnimationScript vegaAnimScript;
    [HideInInspector] public PlayerAnimationScript currentAnimationScript;
    
    [Header("Stats and UI")]
    private bool overwriteOnline;
    [HideInInspector] public Camera _camera;

    [Header("State")]
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int currentShield;
    [HideInInspector] public bool isCasting;
    [HideInInspector] public bool canMove;
    
    [Header("Attack")]
    [HideInInspector] public int baseDamage;
    [HideInInspector] public AnimationCurve damageMultiplier;
    [HideInInspector] public float baseAttackSpeed;
    [HideInInspector] public float attackRange;
    
    [Header("Movement")]
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public AnimationCurve speedCurve;
    [HideInInspector] public AnimationCurve slowDownCurve;
    [HideInInspector] [Range(0, 1)] public float force;
    
    [Header("FX")]
    [HideInInspector] public ParticleSystem attackFx;
    [HideInInspector] public ParticleSystem laserFX;
    [HideInInspector] public GameObject textDamage;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!overwriteOnline)
        {
            GameAdministrator.instance.localViewID = GetComponent<PhotonView>().ViewID; 
            GameAdministrator.instance.localPlayerView = GetComponent<PhotonView>();   
        }

        PlayerController.myTargetable.photonID = photonView.ViewID;
        PlayerController.myTargetable.bodyPhotonID = PlayerController.GetComponent<PhotonView>().ViewID;
        PlayerController.agent = GetComponentInChildren<NavMeshAgent>();
        PlayerController.manager = this;
    }

    private void Start()
    {
        username = photonView.Controller.NickName;
        PlayerController.myTargetable.healthBar.name = username;
        PlayerController.myTargetable.UpdateUI(false,false,0, 0,false,0,true,username);
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

        #region DATA

        if (championDataSo)
        {
            //DATA
            
            //Movement
            currentSpeed = championDataSo.baseSpeed;
            baseSpeed = championDataSo.baseSpeed;
            speedCurve = championDataSo.speedCurve;
            slowDownCurve = championDataSo.slowDownCurve;

            //Attack
            baseDamage = championDataSo.baseDamage;
            damageMultiplier = championDataSo.damageMultiplier;
            baseAttackSpeed = championDataSo.baseAttackSpeed;
            attackRange = championDataSo.attackRange;

            //CAPACITIES
            PlayerController.ActiveCapacity1SO = championDataSo.ActiveCapacity1So;
            PlayerController.ActiveCapacity2SO = championDataSo.ActiveCapacity2So;
            PlayerController.UltimateCapacitySO = championDataSo.UltimateCapacitySo;
        }

        #endregion
        
       

        #region PHOTON

        if (photonView.IsMine)
        {
            PlayerController.enabled = true;
            hud.SetActive(true);
            cam.SetActive(true);
            
            GameAdministrator.instance.localPlayer = this;
        }

        #endregion
        
        playerAnimationScript.SetTeamModel(currentTeam);
    }

    public void SetPlayerActiveState(bool isActive)
    {
        playerAnimationScript.SetTeamModel(currentTeam);
        modelBody.SetActive(isActive);
    }

    private void LateUpdate()
    {
        SetUI();
    }

    void SetUI()
    {
        fogOfWarRenderer.position = PlayerController.transform.position;
        PlayerController.myTargetable.UpdateUI(true,true,currentHealth, maxHealth);
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

    public void HitStop(int[] targetsID,float time,float shakeForce)
    {
        Hashtable data = new Hashtable
        {
            {"TargetsID", targetsID},
            {"Time", time},
            {"Force", shakeForce}
        };
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};

        PhotonNetwork.RaiseEvent(RaiseEvent.HitStopTarget, data, raiseEventOptions, SendOptions.SendReliable);
    }
    
    public void KnockBack(int[] targetsID,float time,float force,Vector3 direction)
    {
        Hashtable data = new Hashtable
        {
            {"TargetsID", targetsID},
            {"Time", time},
            {"Force", force},
            {"Direction", direction}
        };
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};

        PhotonNetwork.RaiseEvent(RaiseEvent.KnockBackTarget, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendPlayerCharacter(int characterID)
    {
        Hashtable data = new Hashtable()
        {
            { "ID", photonView.ViewID },
            { "CharacterID", characterID }
        };
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};

        PhotonNetwork.RaiseEvent(RaiseEvent.SetCharacter, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendPlayerTeam(int team)
    {
        Hashtable data = new Hashtable()
        {
            { "ID", photonView.ViewID },
            { "TeamID", team }
        };
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};

        PhotonNetwork.RaiseEvent(RaiseEvent.SetTeam, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void ApplyCharacterChoice(Enums.Character character)
    {
        switch (character)
        {
            case Enums.Character.Mimi:
                vegaModel.SetActive(false);
                mimiModel.SetActive(true);
                currentAnimationScript = mimiAnimScript;
                break;
            
            case Enums.Character.Vega:
                mimiModel.SetActive(false);
                vegaModel.SetActive(true);
                currentAnimationScript = vegaAnimScript;
                break;
        }
    }

    public void ApplyTeamChoice(Enums.Teams team)
    {
        SetTeam(team);
    }
    
    public void OnEvent(EventData photonEvent)
    {
        Hashtable data = (Hashtable)photonEvent.CustomData;
        if (data == null) return;

            Debug.Log((int)data["ID"]);
            
            if (photonEvent.Code == RaiseEvent.SetCharacter)
            {
                int characterID = (int)data["CharacterID"];
                int playerID = (int)data["ID"];

                Debug.Log(photonView.ViewID + " + " + playerID);

                if(photonView.ViewID != playerID) return;
                
                switch (characterID)
                {
                    case 0 :
                        //MIMI
                        ApplyCharacterChoice(Enums.Character.Mimi);
                        Debug.Log(photonView.ViewID + " selected Mimi");
                        break;
                    
                    case 1:
                        //VEGA
                        ApplyCharacterChoice(Enums.Character.Vega);
                        Debug.Log(photonView.ViewID + " selected Vega");
                        break;
                }
                return;
            }
            
            if (photonEvent.Code == RaiseEvent.SetTeam)
            {
                int teamID = (int)data["TeamID"];
                int playerID = (int)data["ID"];

                Debug.Log(photonView.ViewID + " + " + playerID);

                if(photonView.ViewID != playerID) return;
                
                switch (teamID)
                {
                    case 0:
                        ApplyTeamChoice(Enums.Teams.Green);
                        Debug.Log(photonView.ViewID + " selected team Green");
                        break;
                    case 1:
                        ApplyTeamChoice(Enums.Teams.Pink);
                        Debug.Log(photonView.ViewID + " selected team Pink");
                        break;
                }
                return;
            }

        int[] targets = (int[])data["TargetsID"];
        if(targets == null) return;
        
        foreach (int id in targets)
        {
            if (photonView.ViewID == id)
            {
                if (photonEvent.Code == RaiseEvent.DamageTarget)
                {
                    TakeDamage(data);
                }

                if (photonEvent.Code == RaiseEvent.KnockBackTarget)
                {
                    InitializeKnockBack(data);
                }
                
                if (photonEvent.Code == RaiseEvent.HitStopTarget)
                {
                    InitializeHitStop(data);
                }

                if (photonEvent.Code == RaiseEvent.Death)
                {
                    Death();
                }
            }
        }
    }

    public void InitializeHitStop(Hashtable data)
    {
        float time = (float) data["Time"];
        float shakeForce = (float) data["Force"];
        PlayerController.HitStop(time);
        
        if (shakeForce > 0)
        {
            playerAnimationScript.Shake(shakeForce,time);
        }
    }
    
    public void InitializeKnockBack(Hashtable data)
    {
        float time = (float) data["Time"];
        float force = (float) data["Force"];
        Vector3 direction = (Vector3) data["Direction"];
        PlayerController.InitializeKnockBack(time,force,direction);
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
                GameObject textDmg = Instantiate(textDamage, PlayerController.myTargetable.healthBar.transform.position + Vector3.up * 30, quaternion.identity, MobsUIManager.instance.canvas);
                textDmg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "-" + amount;
            }
        }
        else
        {
            currentHealth -= amount;
            GameObject textDmg = Instantiate(textDamage, PlayerController.myTargetable.healthBar.transform.position + Vector3.up * 30, quaternion.identity, MobsUIManager.instance.canvas);
            textDmg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "-" + amount;
        }
        
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
        LevelManager levelManager = FindObjectOfType<LevelManager>();

        if (levelManager.gameStarted)
        {
            switch (currentTeam)
            {
                case Enums.Teams.Green:
                    PlayerController.transform.position = levelManager.spawnPointGreenA.position;
                    break;
                case Enums.Teams.Pink:
                    PlayerController.transform.position = levelManager.spawnPointPinkA.position;
                    break;
            }
        }
        else
        {
            PlayerController.transform.position = levelManager.hubSpawnPoint.position;

        }
        
        PlayerController.agent.ResetPath();
    }

    public Enums.Teams CurrentTeam()
    {
        return currentTeam;
    }

    public void SetTeam(Enums.Teams team)
    {
        currentTeam = team;
    }
}

public enum VisualEffects
{
    MimiAutoAttack,
    MimiLaser
}

public enum Capacities
{
    MIMI_Laser,
    MIMI_Ultimate,
    VEGA_Blackhole,
    VEGA_Ultimate
}
