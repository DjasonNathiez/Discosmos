using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool clientPlayer;
    public PlayerManager playerManager;

    [Header("Inputs")] 
    public KeyCode activeCapacity1 = KeyCode.A;
    public KeyCode activeCapacity2 = KeyCode.Z;
    public KeyCode ultimateCapacity = KeyCode.E;
    
    [Header("Capacities")] 
    public PassiveCapacity PassiveCapacity;
    public ActiveCapacitySO ActiveCapacity1SO;
    private ActiveCapacity ActiveCapacity1;
    public ActiveCapacitySO ActiveCapacity2SO;
    private ActiveCapacity ActiveCapacity2;
    public ActiveCapacitySO UltimateCapacitySO;
    private ActiveCapacity UltimateCapacity;

    public delegate void OnCastEnded(Capacities capacity);
    public OnCastEnded OnCastEnd;

    [Header("Auto Attack")] 
    
    public int baseDamages;
    public AnimationCurve damageMultiplier;
    public float range;
    public PlayerManager cible;
    public bool isAttacking;

    [Header("Movement")]
    
    [SerializeField] private MovementType movementType = MovementType.MoveToClickWithNavMesh;
    [Range(0, 1)] public float force; //from 0 to 1
    [Range(0, 20)] [SerializeField] public float baseSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve slowDownCurve;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool moving;

    [Header("Ramps")] 
    
    [SerializeField] public bool onRamp;
    [SerializeField] public int rampIndex;
    [SerializeField] private Rampe_LD ramp;
    [SerializeField] public float rampProgress;
    [SerializeField] public bool forwardOnRamp;
    
    
    private Ray ray;
    private RaycastHit hit;
    private Vector3 destination;
    private Vector3 direction;

    private bool attacking = false;

    private bool speedPadTriggered = false;
    private Transform speedPad;
    
    [Header("SpeedPads")]
    
    [SerializeField] private float precisionAnglePad;
    [SerializeField] private float slowSpeedPadDuration = 0.5f;
    [SerializeField] private float fastSpeedPadDuration = 2;
    private float forcePad = 0;
    private double speedPadTimer = 0;
    private double time;
    [SerializeField] private float speedPadLerp = 1;

    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sparkles;

    [Header("UI")] 
    [SerializeField] private Image speedJauge;

    private void OnEnable()
    {
        OnCastEnd += OnCapacityActive;
        
        ActiveCapacity1SO.GetActiveCapacity();
        ActiveCapacity1 = ActiveCapacity1SO.activeCapacity;
        ActiveCapacity1.InitializeCapacity(ActiveCapacity1SO);
        ActiveCapacity1.owner = this;
        
        ActiveCapacity2SO.GetActiveCapacity();
        ActiveCapacity2 = ActiveCapacity2SO.activeCapacity;
        ActiveCapacity2.InitializeCapacity(ActiveCapacity2SO);
        ActiveCapacity2.owner = this;
        
        UltimateCapacitySO.GetActiveCapacity();
        UltimateCapacity = UltimateCapacitySO.activeCapacity;
        UltimateCapacity.InitializeCapacity(UltimateCapacitySO);
        UltimateCapacity.owner = this;
    }

    private void OnDisable()
    {
        
        OnCastEnd -= OnCapacityActive;
        
        ActiveCapacity1 = null;
        ActiveCapacity2 = null;
        UltimateCapacity = null;

    }

    private void Start()
    {
        if (clientPlayer)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = currentSpeed;
            //if photon is on, then we are in multiplayer
            SetTime();   
        }
    }

    private void SetTime()
    {
        if (PhotonNetwork.IsConnected)
        {
            time = PhotonNetwork.Time;
        }
        else
        {
            time = Time.deltaTime;
        }
    }

    private void Update()
    {
        if (clientPlayer)
        {
            SetTime();

            CapacitiesInputCheck();
            AttackCheck();
            MovementTypeSwitch();
            playerManager.speedBar.fillAmount = force;
            currentSpeed = speedCurve.Evaluate(force) + baseSpeed;
            agent.speed = currentSpeed;

            Debug.DrawLine(transform.position, agent.destination, Color.yellow);

            SpeedPadFunction();   
        }
    }

    #region SpeedPad

    private void SpeedPadFunction()
    {
        if (speedPadTriggered)
        {
            SpeedPadEffect();
            speedPadTimer = 0;
        }

        if (!speedPadTriggered && forcePad != 0)
        {
            speedPadTimer += time;
            if (forcePad > 0)
            {
                if (speedPadTimer >= fastSpeedPadDuration)
                {
                    GoBackToNormalSpeed();
                }
            }
            else
            {
                if (speedPadTimer >= slowSpeedPadDuration)
                {
                    GoBackToNormalSpeed();
                }
            }
        }
    }

    private void GoBackToNormalSpeed()
    {
        forcePad = Mathf.Lerp(forcePad, 0, speedPadLerp);
    }

    public void SpeedPadTrigger(bool isTriggered, Transform speedPadTransform)
    {
        speedPadTriggered = isTriggered;
        if (speedPadTriggered)
        {
            speedPad = speedPadTransform;
        }
        else
        {
            speedPad = null;
        }
    }

    //if the speed pad is triggered get the transform of the speed pad
    private void SpeedPadEffect()
    {
        //if the player is going approximately in the same direction as the speed pad and not in the opposite direction
        if (Vector3.Angle(transform.forward, speedPad.forward) < precisionAnglePad)
        {
            SpeedUpPad();
        }
        else
        {
            SlowDownPad();
        }
    }

    private void SlowDownPad()
    {
        forcePad = Mathf.Lerp(forcePad, 0, speedPadLerp);
    }

    private void SpeedUpPad()
    {
        forcePad = Mathf.Lerp(forcePad, 1, speedPadLerp);;
    }

    #endregion


    private enum MovementType
    {
        MoveToClickWithNavMesh,
        KeepDirectionWithoutNavMesh,
        Slide,
        FollowCible,
        Attack
    }


    private void MovementTypeSwitch()
    {
        if (playerManager._camera != null) ray = playerManager._camera.ScreenPointToRay(Input.mousePosition);
        switch (movementType)
        {
            case MovementType.MoveToClickWithNavMesh:
                MoveToClickUsingTheNavMesh();
                break;
            case MovementType.KeepDirectionWithoutNavMesh:
                KeepDirectionNoNavMesh();
                break;
            case MovementType.Slide:
                Slide();
                break;
            case MovementType.FollowCible:
                FollowCible();
                break;
            case MovementType.Attack:
                Attack();
                break;
        }
    }

    void AttackCheck()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if(cible) cible.HideTarget();
                    cible = hit.transform.GetComponent<PlayerController>().playerManager;
                    cible.ShowTarget();
                    if (onRamp)
                    {
                        OnExitRamp();
                    }
                    movementType = MovementType.FollowCible;
                    ChangeAnimation(force <= 0 ? 1 : 2);
                }
            }
        }
    }

    void CapacitiesInputCheck()
    {
        if (Input.GetKeyUp(activeCapacity1))
        {
            ActiveCapacity1.Cast();
        }

        if (Input.GetKeyUp(activeCapacity2))
        {
            ActiveCapacity2.Cast();
        }

        if (Input.GetKeyUp(ultimateCapacity))
        {
            UltimateCapacity.Cast();
        }
    }

    void OnCapacityActive(Capacities capacity)
    {
        switch (capacity)
        {
            case Capacities.MIMI_Laser:
                Debug.Log("Perform the mimi laser at " + PhotonNetwork.Time);
                OnCapacityPerformed(Capacities.MIMI_Laser);
                break;
        }
    }
    
    public void OnCapacityPerformed(Capacities capacity)
    {
        switch (capacity)
        {
            case Capacities.MIMI_Laser:
                int damages = Mathf.RoundToInt(ActiveCapacity1.activeCapacitySo.amount * damageMultiplier.Evaluate(force));
                playerManager.DealDamage(ActiveCapacity1.GetTargets(transform.forward), damages);
                break;
        }
    }

    public void OnAttack()
    {
        int damages = Mathf.RoundToInt(baseDamages * damageMultiplier.Evaluate(force));
        force = 0;
        playerManager.DealDamage(new []{cible.GetComponent<PhotonView>().ViewID}, damages);
    }

    void FollowCible()
    {
        agent.ResetPath();
        agent.SetDestination(cible.PlayerController.transform.position);
        force -= slowDownCurve.Evaluate(force) * Time.deltaTime;
        force = Mathf.Clamp01(force);
        agent.enabled = true;
        if (moving && agent.remainingDistance == 0)
        {
            moving = false;
            ChangeAnimation(0);
        }

        if (Vector3.SqrMagnitude(cible.PlayerController.transform.position - transform.position) <= range * range)
        {
            agent.ResetPath();
            movementType = MovementType.Attack;
        }
        

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                ResetTarget();
                agent.SetDestination(hit.point);
                movementType = MovementType.MoveToClickWithNavMesh;
                moving = true;
                ChangeAnimation(force <= 0 ? 1 : 2);
            }
        }
    }

    public void ChangeAnimation(int index)
    {
        Debug.Log(index);
        animator.SetInteger("Animation",index);
    }

    public void ResetTarget()
    {
        if(cible) cible.HideTarget();
        cible = null;
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            ChangeAnimation(4);
            isAttacking = true;
        }
        else
        {
            force -= slowDownCurve.Evaluate(force) * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(cible.PlayerController.transform.position - transform.position);
            
            if (Vector3.SqrMagnitude(cible.PlayerController.transform.position - transform.position) > range * range)
            {
                isAttacking = false;
                movementType = MovementType.FollowCible;
                ChangeAnimation(force <= 0 ? 1 : 2);
            }
            
            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    ResetTarget();
                    isAttacking = false;
                    movementType = MovementType.MoveToClickWithNavMesh;
                    agent.ResetPath();
                    agent.SetDestination(hit.point);
                    moving = true;
                    ChangeAnimation(force <= 0 ? 1 : 2);
                }
            }
        }
    }

    public void OnEnterRamp(Rampe_LD rampeLd,bool forward,int startIndex)
    {
        ResetTarget();
        onRamp = true;
        forwardOnRamp = forward;
        rampIndex = startIndex;
        rampProgress = 0;
        ramp = rampeLd;
        movementType = MovementType.Slide;
        agent.ResetPath();
        ChangeAnimation(3);
        sparkles.SetActive(true);
    }

    public void Slide()
    {
        if (rampProgress < 1)
        {
            rampProgress += (Time.deltaTime * currentSpeed) / ramp.distBetweenNodes;
        }
        else
        {
            if (forwardOnRamp)
            {
                rampIndex++;
                if (rampIndex == ramp.distancedNodes.Count - 1)
                {
                    direction = ramp.exitDirectionLastNode.normalized;
                    OnExitRamp();
                    return;
                }
            }
            else
            {
                rampIndex--;
                if (rampIndex == 0)
                {
                    direction = ramp.exitDirectionFirstNode.normalized;
                    OnExitRamp();
                    return;
                }
            }
            rampProgress = 0;
        }

        if (onRamp)
        {
            transform.position = Vector3.Lerp(ramp.distancedNodes[rampIndex], ramp.distancedNodes[forwardOnRamp ? rampIndex + 1 : rampIndex -1], rampProgress) + Vector3.up * ramp.heightOnRamp;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(ramp.distancedNodes[forwardOnRamp ? rampIndex + 1 : rampIndex -1] - ramp.distancedNodes[rampIndex]),Time.deltaTime*8);
            force += ramp.speedBoost.Evaluate(force) * Time.deltaTime;
        }
    }

    void OnExitRamp()
    {
        ChangeAnimation(force <= 0 ? 1 : 2);
        onRamp = false;
        movementType = MovementType.KeepDirectionWithoutNavMesh;
        ramp.OnExitRamp();
        sparkles.SetActive(false);
    }

    private void KeepDirectionNoNavMesh()
    {
        transform.position += direction * (currentSpeed * Time.deltaTime);
        force -= slowDownCurve.Evaluate(force) * Time.deltaTime;
        force = Mathf.Clamp01(force);
        if (force <= 0)
        {
            agent.ResetPath();
            movementType = MovementType.MoveToClickWithNavMesh;
            ChangeAnimation(0);
        }
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                agent.SetDestination(hit.point);
                movementType = MovementType.MoveToClickWithNavMesh;
            }
        }
    }


    private void MoveToClickUsingTheNavMesh()
    {
        force -= slowDownCurve.Evaluate(force) * Time.deltaTime;
        force = Mathf.Clamp01(force);
        agent.enabled = true;
        if (moving && agent.remainingDistance == 0)
        {
            moving = false;
            ChangeAnimation(0);
        }
        
        
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                agent.SetDestination(hit.point);
                moving = true;
                ChangeAnimation(force <= 0 ? 1 : 2);
            }
        }
    }
}