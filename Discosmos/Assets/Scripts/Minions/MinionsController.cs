using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Toolbox.Variable;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MinionsController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private bool master;
    public Targetable myTargetable;
    public Transform renderBody;

    public int currentHealth;
    public int currentShield;
    public int maxHealth;

    private NavMeshAgent agent;
    public int currentWaypoint = 0;
    private bool isMoving = false;
    private bool isAttacking = false;
        
    private List<GameObject> entitiesInRange = new List<GameObject>();
    public GameObject target;
    private Vector3 targetPosition;
    
    private Collider[] colliders;
    private GameObject waypoints;
    
    [SerializeField] private bool loopMode;
    
    [Header("HIT STOP AND SHAKING")]
    public bool shaking;
    public float shakingForce;
    public float shakingTime;
    public float shakingDuration;
    public Vector3 truePos;
    public float previousShake = 1;
    public float nextShakeTimer = 0.02f;
    public float shakeFrequency = 0.02f;
    public bool  movementEnabled = false;
    
    
    [Header("KNOCKBACK")]
    [SerializeField] private Vector3 knockbackDirection;
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockBackTime;
    [SerializeField] private float knockBackDuration;
    
    
    
    private enum FollowType
    {
        NearToFar,
        OrderedList
    }
    
    [SerializeField] private FollowType followType;

    private void Start()
    {
        currentHealth = maxHealth;
        
        if (PhotonNetwork.IsMasterClient)
        {
            agent = GetComponent<NavMeshAgent>();
            //Find the gameObject named "WaypointsTeam" + team.teamID and add the transforms of its children to the waypoints array
            waypoints = GameObject.Find("WaypointsTeam" + 0);
            _waypoints = waypoints.GetComponentsInChildren<Transform>();
            _waypoints = _waypoints[1..];
            ChooseTypeOfFollow();
            master = true;
        }
        else
        {
            agent = agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
        }

        myTargetable.photonID = photonView.ViewID;
        myTargetable.bodyPhotonID = photonView.ViewID;
        truePos = renderBody.transform.localPosition;
    }

    private void FollowFromNearestToFarthest()
    {
        
        System.Array.Sort(_waypoints,
            (x, y) => Vector3.Distance(transform.position, x.position)
                .CompareTo(Vector3.Distance(transform.position, y.position)));
    }

    private void ChooseTypeOfFollow()
    {
        switch (followType)
        {
            case FollowType.NearToFar:
                FollowFromNearestToFarthest();
                break;
            case FollowType.OrderedList:
                break;
        }
    }

    private void Update()
    {
        if (master)
        {
            if (shaking)
            {
                ApplyShaking();
            }

            if (!movementEnabled)
            {
                return;
            }
            
            if (knockBackTime > 0)
            { 
                ApplyKnockBack();
            }
            
            //while there are no GameObjects with the team != this.team in the entitiesInRange list and the currentWaypoint is not the last waypoint move to the next waypoint else move In Range and attack
            if (entitiesInRange.Count == 0 && currentWaypoint < _waypoints.Length)
            {
                MoveToWaypoint();
            }
            else if (entitiesInRange.Count == 0 && currentWaypoint == _waypoints.Length)
            {
                if (loopMode)
                {
                    currentWaypoint = 0;
                    MoveToWaypoint();
                }
                else
                {
                    agent.isStopped = true;
                }
            }
            else
            {
                MoveInRange();
            }   
        }
    }

    private void LateUpdate()
    {
        myTargetable.UpdateUI(true,true,currentHealth, maxHealth);
    }

    private void MoveToWaypoint()
    {
        agent.SetDestination(_waypoints[currentWaypoint].position);
        if (Vector3.SqrMagnitude(new Vector3(transform.position.x,0,transform.position.z) - new Vector3(_waypoints[currentWaypoint].position.x,0,_waypoints[currentWaypoint].position.z)) < 1f)
        {
            currentWaypoint++;
        }

    }
    
    private void MoveInRange()
    {
        colliders = Physics.OverlapSphere(transform.position, range);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<Team>() != null)
            {
                if (collider.gameObject.GetComponent<Team>().TeamID != 0)
                {
                    target = collider.gameObject;
                    targetPosition = target.transform.position;
                    agent.SetDestination(targetPosition);
                    if (Vector3.Distance(transform.position, targetPosition) < 1f)
                    {
                        Attack();
                    }
                }
            }
        }
    }
    
    public void InitializeHitStop(float time,float force)
    {
        shakingForce = force;
        shakingTime = time;
        shakingDuration = time;
        shaking = true;
        movementEnabled = false;
        agent.isStopped = true;
    }
    
    public void InitializeKnockBack(float kbTime,float kbForce, Vector3 kbDirection)
    {
        knockbackDirection = kbDirection;
        knockBackDuration = kbTime;
        knockBackTime = kbTime;
        knockBackForce = kbForce;
    }
    
    private void ApplyKnockBack()
    {
        agent.nextPosition += knockbackDirection * (knockBackForce * Time.deltaTime * (knockBackTime / knockBackDuration));
        knockBackTime -= Time.deltaTime;
    }

    void ApplyShaking()
    {
        if (shaking)
        {
            if (shakingTime > 0)
            {
                shakingTime -= Time.deltaTime;
                if (nextShakeTimer > 0)
                {
                    nextShakeTimer -= Time.deltaTime;
                }
                else
                {
                    nextShakeTimer = shakeFrequency;
                    Vector2 shake = new Vector2(Random.Range(0.2f, 1f) * shakingForce * (shakingTime /(shakingDuration / 0.7f) + 0.3f)* previousShake, Random.Range(0.2f, 1f) * shakingForce * (shakingTime /(shakingDuration / 0.7f) + 0.3f) * previousShake);
                    renderBody.localPosition = new Vector3(truePos.x + shake.x, truePos.y, truePos.z + shake.y);
                    previousShake = -previousShake;
                }
            }
            else
            {
                renderBody.localPosition = truePos;
                shakingTime = 0;
                shaking = false;
                movementEnabled = true;
                agent.isStopped = false;
            }   
        }
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }

    public void DealDamage(int[] targetsID, int damageAmount)
    {
        Hashtable data = new Hashtable
        {
            {"TargetsID", targetsID},
            {"Amount", damageAmount}
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };

        PhotonNetwork.RaiseEvent(RaiseEvent.DamageTarget, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == RaiseEvent.DamageTarget)
        {
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
        
        if (photonEvent.Code == RaiseEvent.HitStopTarget)
        {
            
            Hashtable data = (Hashtable)photonEvent.CustomData;
            int[] targets = (int[])data["TargetsID"];

            foreach (int id in targets)
            {
                if (photonView.ViewID == id)
                {
                    InitializeHitStop((float)data["Time"],(float)data["Force"]);
                }
            }   
            
        }
        
        if (photonEvent.Code == RaiseEvent.KnockBackTarget)
        {
            if (master)
            {
                Hashtable data = (Hashtable) photonEvent.CustomData;
                int[] targets = (int[]) data["TargetsID"];

                foreach (int id in targets)
                {
                    if (photonView.ViewID == id)
                    {
                        InitializeKnockBack((float) data["Time"], (float) data["Force"], (Vector3) data["Direction"]);
                    }
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

    private void Death()
    {
        //gameObject.SetActive(false);
    }

    private void TakeDamage(Hashtable data)
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

        if(currentHealth <= 0)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, };
            PhotonNetwork.RaiseEvent(RaiseEvent.Death, new Hashtable{{"ID", photonView.ViewID}}, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}