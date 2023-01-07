using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    private Rigidbody rb;
    [HideInInspector] public bool clientPlayer;
    [HideInInspector] public PlayerManager manager;

    [Header("Inputs")] 
    public KeyCode activeCapacity1 = KeyCode.A;
    public KeyCode activeCapacity2 = KeyCode.Z;
    public KeyCode ultimateCapacity = KeyCode.E;
    
    [Header("Capacities")]
    
    [HideInInspector]public PassiveCapacitySO PassiveCapacitySO;
    public ActiveCapacitySO ActiveCapacity1SO;
    [HideInInspector]public ActiveCapacitySO ActiveCapacity2SO;
    [HideInInspector]public ActiveCapacitySO UltimateCapacitySO;
    
    private PassiveCapacity PassiveCapacity;
    public ActiveCapacity ActiveCapacity1;
    private ActiveCapacity ActiveCapacity2;
    private ActiveCapacity UltimateCapacity;

    public delegate void CastEnded(Capacities capacity);
    public CastEnded OnCastEnded;

    public GameObject mimiLaserVisualization;
    public GameObject vegaBlackHoleVisual;
    public Vegas_Black_Hole blackHole;

    [Header("Auto Attack")] 
    
    private Targetable cible;
    [HideInInspector] public Targetable myTargetable;
    private bool isAttacking;

    [Header("Movement")]
    private MovementType movementType = MovementType.MoveToClickWithNavMesh;
    private bool moving;
    private bool movementEnabled = true;
    private float hitStopTime;

    [Header("Ramps")]
    private bool onRamp;
    [HideInInspector] public int rampIndex;
    private Rampe_LD ramp;
    [HideInInspector] public float rampProgress;
    [HideInInspector] public bool forwardOnRamp;
    
    private Vector3 destination;
    private Vector3 direction;
    
    private Vector3 knockbackDirection;
    private float knockBackForce;
    private float knockBackTime;
    private float knockBackDuration;
    private bool knockBackImmediatly;

    private bool attacking = false;

    [Header("SpeedPads")]
    private bool speedPadTriggered = false;
    private Transform speedPad;
    private float precisionAnglePad;
    private float slowSpeedPadDuration = 0.5f;
    private float fastSpeedPadDuration = 2;
    private float forcePad = 0;
    private double speedPadTimer = 0;
    private double time;
   private float speedPadLerp = 1;

    [Header("Animation")] 
    public Animator animatorMimi;
    public Animator animatorVega;
    [SerializeField] private GameObject sparkles;

    [Header("UI")] 
    private Image speedJauge;


    #region INITIALIZATION

    private void OnEnable()
    {
        InitCapacities();
    }

    public void InitCapacities()
    {
        OnCastEnded += OnCapacityActive;
        
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
        
        OnCastEnded -= OnCapacityActive;
        
        ActiveCapacity1 = null;
        ActiveCapacity2 = null;
        UltimateCapacity = null;

    }

    #endregion

    #region UNITY CALLBACKS

    private void Start()
    {
        if (clientPlayer)
        {
            agent.speed = manager.currentSpeed;
            rb = GetComponent<Rigidbody>();
            //if photon is on, then we are in multiplayer
            SetTime();   
        }
    }

    #endregion
    
    private void SetTime()
    {
        time = PhotonNetwork.IsConnected ? PhotonNetwork.Time : Time.deltaTime;
    }

    private void Update()
    {
        if (clientPlayer)
        {

            if (Input.GetKeyDown(KeyCode.K))
            {
                manager.HitStop(new []{myTargetable.photonID}, 0.7f,0.3f);
                manager.KnockBack(new []{myTargetable.photonID}, 0.45f ,9f ,Vector3.left);
            }
            SetTime();

            CapacitiesInputCheck();
            AttackCheck();
            MovementTypeSwitch();
            myTargetable.UpdateUI(false,false,0,0,true,Mathf.Lerp(myTargetable.healthBar.speedFill.fillAmount,manager.force,Time.deltaTime * 5f));
            manager.currentSpeed = manager.speedCurve.Evaluate(manager.force) + manager.baseSpeed;
            agent.speed = manager.currentSpeed;
            if (manager.currentData == manager.mimiData)
            {
                animatorMimi.SetFloat("Speed",manager.force*1.5f+1);
            }
            else
            {
                animatorVega.SetFloat("Speed",manager.force*1.5f+1);   
            }
            SpeedPadFunction();
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity != Vector3.zero)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5f);
        }
        if (rb.velocity.magnitude < 0.5f)
        {
            rb.velocity = Vector3.zero;
        }
        if (rb.angularVelocity != Vector3.zero)
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 5f);
        }
        if (rb.angularVelocity.magnitude < 0.5f)
        {
            rb.angularVelocity = Vector3.zero;
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
        Attack,
        Block
    }

    #region INPUT

    public Vector3 MouseWorldPosition()
    {
        Ray ray = manager._camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }
    
    public Targetable GetTarget()
    {
        if (manager._camera == null) return null;
        
        Ray ray = manager._camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Targetable targetable = hit.transform.GetComponentInParent<Targetable>();
            ITeamable teamable = hit.transform.GetComponentInParent<ITeamable>();

            if (targetable == null || targetable == myTargetable) return null;

            if (teamable != null)
            {
                if (manager.CurrentTeam() == teamable.CurrentTeam()) return null;
            }
            if (manager.currentData == manager.mimiData)
            {
                animatorMimi.SetInteger("Target",targetable.bodyPhotonID);   
            }
            else
            {
                animatorVega.SetInteger("Target",targetable.bodyPhotonID);   
            }
            return targetable;
        }
        return null;
    }
    

    #endregion
    
    #region COMBAT

    void AttackCheck()
    {
        if (Input.GetMouseButton(0))
        {
            if (cible)
            {
                cible.HideTarget();
            }

            cible = GetTarget();


            if (cible)
            {
                cible.ShowTarget();

                if (onRamp)
                {
                    OnExitRamp();
                }

                movementType = MovementType.FollowCible;
                ChangeAnimation(manager.force <= 0 ? 1 : 2);
            }
            else
            {
                movementType = MovementType.MoveToClickWithNavMesh;
                ChangeAnimation(0); 
            } 
        }
        
    }
    
    private void Attack()
    {
        if (!isAttacking)
        {
            if (Vector3.SqrMagnitude(cible.targetableBody.transform.position - transform.position) > manager.attackRange * manager.attackRange)
            {
                isAttacking = false;
                movementType = MovementType.FollowCible;
                ChangeAnimation(manager.force <= 0 ? 1 : 2);
            }
            else
            {
                ChangeAnimation(4);
                isAttacking = true;
                if(manager.force > 0) InitializeKnockBack(0.1f,manager.speedCurve.Evaluate(manager.force),cible.targetableBody.position - transform.position,true);
            }
        }
        else
        {
            manager.force -= manager.slowDownCurve.Evaluate(manager.force) * Time.deltaTime;

            
            if(!manager.isCasting) transform.rotation = Quaternion.LookRotation(cible.targetableBody.position - transform.position);

            if (manager.currentData == manager.mimiData && animatorMimi.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
            }
            else if (manager.currentData == manager.vegaData && animatorVega.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
            }
            
            if (Input.GetMouseButton(1))
            {
                ResetTarget();
                isAttacking = false;
                movementType = MovementType.MoveToClickWithNavMesh;
                agent.ResetPath();
                agent.SetDestination(MouseWorldPosition());
                moving = true;
                ChangeAnimation(manager.force <= 0 ? 1 : 2);
            }
        }
    }

    public void OnAttack()
    {
        if (enabled)
        {
            int damages = Mathf.RoundToInt(manager.baseDamage * manager.damageMultiplier.Evaluate(manager.force));

            manager.DealDamage(new []{cible.photonID}, damages);
            manager.HitStop(new []{cible.photonID}, manager.force > 0 ? 0.7f * manager.force : 0,manager.force > 0 ? 0.3f * manager.force : 0);
            Vector3 kbDirection = cible.targetableBody.position - transform.position;
            manager.KnockBack(new []{cible.photonID}, manager.force > 0 ? 0.45f * manager.force : 0,manager.force > 0 ? 9f * manager.force : 0,kbDirection.normalized);
            manager.force = 0;
        }
    }

    
    public void ResetTarget()
    {
        if (cible)
        {
            cible.HideTarget();
        }
        
        cible = null;
    }
    
    public void HitStop(float time)
    {
        Debug.Log("hitStop " + time);
        movementEnabled = false;
        hitStopTime = time;
        agent.isStopped = true;
    }

    void HitStopTimer()
    {
        if (hitStopTime > 0)
        {
            hitStopTime -= Time.deltaTime;
        }
        else
        {
            movementEnabled = true;
            agent.isStopped = false;
        }
    }

    #endregion
    
    #region CAPACITIES

    void CapacitiesInputCheck()
    {
        if(manager.isCasting) return;
        
        if (Input.GetKeyDown(activeCapacity1) && !ActiveCapacity1.onCooldown)
        {
            mimiLaserVisualization.SetActive(true); 
        }
        
        if (Input.GetKey(activeCapacity1))
        {
            mimiLaserVisualization.transform.rotation = Quaternion.Euler(0,Quaternion.LookRotation(MouseWorldPosition()- transform.position).eulerAngles.y,0);
        }
        
        if (Input.GetKeyUp(activeCapacity1) && !ActiveCapacity1.onCooldown)
        {
            if (onRamp)
            {
                OnExitRamp();
            }
            
            StopMovement();
            
            //Laser Mimi
            transform.rotation = Quaternion.Euler(0,Quaternion.LookRotation(MouseWorldPosition()- transform.position).eulerAngles.y,0);
            manager.isCasting = true;
            ActiveCapacity1.Cast();

            if (manager.currentAnimationScript == manager.mimiAnimScript)
            {
                Debug.Log("laser performed");
                mimiLaserVisualization.SetActive(false);
            }
            else
            {
                manager.SendInput(manager.photonView.ViewID, 1);
            }
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

    public void LoadBlackHole()
    {
        Debug.Log("Black hole performed");
        vegaBlackHoleVisual.SetActive(true);
        vegaBlackHoleVisual.transform.position = this.transform.position;
        int damage = Mathf.RoundToInt(ActiveCapacity1SO.amount * manager.damageMultiplier.Evaluate(manager.force));
                
        blackHole.SetBlackHole(transform.forward,  ActiveCapacity1SO.durationBase, ActiveCapacity1SO.speedBase, manager, damage);
    }

    void OnCapacityActive(Capacities capacity)
    {
        switch (capacity)
        {
            case Capacities.MIMI_Laser:
                ChangeAnimation(6);
                break;
            
            case Capacities.VEGA_Blackhole:
                vegaBlackHoleVisual.SetActive(true);
                vegaBlackHoleVisual.transform.position = this.transform.position;
                int damage = Mathf.RoundToInt(ActiveCapacity1SO.amount * manager.damageMultiplier.Evaluate(manager.force));
                
                transform.rotation = Quaternion.Euler(0,Quaternion.LookRotation(MouseWorldPosition()- transform.position).eulerAngles.y,0);
                blackHole.SetBlackHole(transform.forward,  ActiveCapacity1SO.durationBase, ActiveCapacity1SO.speedBase, manager, damage);
                break;
        }
    }
    
    public void OnCapacityPerformed(Capacities capacity, List<int> targets = null)
    {
        manager.isCasting = false;
        
        List<int> enemies = new List<int>();

        if (targets != null)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                ITeamable teamable = PhotonView.Find(targets[i]).GetComponent<ITeamable>();

                if (teamable != null)
                {
                    if (manager.CurrentTeam() != teamable.CurrentTeam())
                    {
                        enemies.Add(targets[i]);
                    }
                }
            }
        }

        switch (capacity)
        {
            case Capacities.MIMI_Laser:
                int damages = Mathf.RoundToInt(ActiveCapacity1SO.amount * manager.damageMultiplier.Evaluate(manager.force));
                manager.DealDamage(enemies.ToArray(), damages);
                manager.HitStop(enemies.ToArray(), manager.force > 0 ? 0.7f * manager.force + 0.2f: 0.2f,manager.force > 0 ? 0.3f * manager.force + 0.1f: 0.1f);
                Vector3 kbDirection = transform.forward;
                manager.KnockBack(enemies.ToArray(), manager.force > 0 ? 0.6f * manager.force : 0,manager.force > 0 ? 11f * manager.force : 0,kbDirection.normalized);
                EnableMovement();
                break;
            
            case Capacities.VEGA_Blackhole:
                vegaBlackHoleVisual.SetActive(true);
                vegaBlackHoleVisual.transform.position = this.transform.position;
                BlackHoleCapacitySO blackHoleData = (BlackHoleCapacitySO) ActiveCapacity1SO;
                int damage = Mathf.RoundToInt(ActiveCapacity1SO.amount * manager.damageMultiplier.Evaluate(manager.force));
                blackHole.SetBlackHole(transform.forward,  ActiveCapacity1SO.durationBase, ActiveCapacity1SO.speedBase, manager, damage);
                break;
        }
    }

    #endregion
    
    #region VISUAL

    
    public void ChangeAnimation(int index)
    {
        if (manager.currentData == manager.mimiData)
        {
            animatorMimi.SetInteger("Animation",index);
        }
        else
        {
            animatorVega.SetInteger("Animation",index);
        }
    }

    #endregion
    
    #region RAMP

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
        
        if(sparkles) sparkles.SetActive(true);
    }

    public void Slide()
    {
        if (rampProgress < 1)
        {
            rampProgress += (Time.deltaTime * manager.currentSpeed) / ramp.distBetweenNodes;
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
            if(!manager.isCasting) transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(ramp.distancedNodes[forwardOnRamp ? rampIndex + 1 : rampIndex -1] - ramp.distancedNodes[rampIndex]),Time.deltaTime*8);
            manager.force += ramp.speedBoost.Evaluate(manager.force) * Time.deltaTime;
        }
    }

    void OnExitRamp()
    {
        ChangeAnimation(manager.force <= 0 ? 1 : 2);
        onRamp = false;
        movementType = MovementType.KeepDirectionWithoutNavMesh;
        ramp.OnExitRamp();
        sparkles.SetActive(false);
    }

    #endregion
    
    #region KNOCKBACK
    private void ApplyKnockBack()
    {
        agent.nextPosition += knockbackDirection * (knockBackForce * Time.deltaTime * (knockBackTime / knockBackDuration));
        //transform.position += knockbackDirection * (knockBackForce * Time.deltaTime * (knockBackTime / knockBackDuration));
        knockBackTime -= Time.deltaTime;
    }

    public void InitializeKnockBack(float kbTime,float kbForce, Vector3 kbDirection,bool applyImmediatly = false)
    {
        knockbackDirection = kbDirection;
        knockBackDuration = kbTime;
        knockBackTime = kbTime;
        knockBackForce = kbForce;
        knockBackImmediatly = applyImmediatly;
    }
    

    #endregion
    
    #region MOVEMENT
    
    private void MoveToClickUsingTheNavMesh()
    {
        manager.force -= manager.slowDownCurve.Evaluate(manager.force) * Time.deltaTime;
        manager.force = Mathf.Clamp01(manager.force);
        agent.enabled = true;
        if (moving && agent.remainingDistance == 0)
        {
            moving = false;
            agent.ResetPath();
            ChangeAnimation(0);
        }
        
        
        if (Input.GetMouseButton(1))
        {
            agent.ResetPath();
            agent.SetDestination(MouseWorldPosition());
            moving = true;
            ChangeAnimation(manager.force <= 0 ? 1 : 2);
        }
    }
    
    private void KeepDirectionNoNavMesh()
    {
        transform.position += direction * (manager.currentSpeed * Time.deltaTime);
        manager.force -= manager.slowDownCurve.Evaluate(manager.force) * Time.deltaTime;
        manager.force = Mathf.Clamp01(manager.force);
        if (manager.force <= 0)
        {
            agent.ResetPath();
            movementType = MovementType.MoveToClickWithNavMesh;
            ChangeAnimation(0);
        }
        if (Input.GetMouseButton(1))
        {
            agent.ResetPath();
            agent.SetDestination(MouseWorldPosition());
            movementType = MovementType.MoveToClickWithNavMesh;
        }
    }
    
    void FollowTarget()
    {
        agent.ResetPath();


        agent.SetDestination(cible.targetableBody.position);

        manager.force -= manager.slowDownCurve.Evaluate(manager.force) * Time.deltaTime;
        manager.force = Mathf.Clamp01(manager.force);
        agent.enabled = true;
        if (moving && agent.remainingDistance == 0)
        {
            moving = false;
            ChangeAnimation(0);
        }

        
        if (Vector3.SqrMagnitude(cible.targetableBody.position - transform.position) <= manager.attackRange * manager.attackRange)
        {
            agent.ResetPath();
            movementType = MovementType.Attack;
        }


        if (Input.GetMouseButton(1))
        {
            agent.ResetPath();
            ResetTarget();
            agent.SetDestination(MouseWorldPosition());
            movementType = MovementType.MoveToClickWithNavMesh;
            moving = true;
            ChangeAnimation(manager.force <= 0 ? 1 : 2);
        }
    }
    
    private void MovementTypeSwitch()
    {
        if (knockBackTime > 0)
        {
            if(knockBackImmediatly || movementEnabled) ApplyKnockBack();
        }
        
        if (!movementEnabled)
        {
            HitStopTimer();
            return;
        }
        
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
                FollowTarget();
                break;
            case MovementType.Attack:
                Attack();
                break;
            case MovementType.Block:
                BlockPlayer();
                break;
        }
    }

    
    public void BlockPlayer()
    {
        if (movementType == MovementType.MoveToClickWithNavMesh)
        {
            agent.ResetPath();
            moving = false;
            
        }
        else if (movementType == MovementType.FollowCible)
        {
            agent.ResetPath();
            moving = false;
           
        }
        else if (movementType == MovementType.Attack)
        {
            isAttacking = false;
        }
        else if (movementType == MovementType.KeepDirectionWithoutNavMesh)
        {
            
        }
        else if (movementType == MovementType.Slide)
        {
            OnExitRamp();
        }
        movementType = MovementType.Block;
        
    }
    
    private List<Vector3> agentsLastPath = new List<Vector3>();
    private void StopMovement()
    {
        movementEnabled = false;
        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            agentsLastPath.Add(agent.path.corners[i]);
        }
        agent.ResetPath();
        moving = false;
    }
    
    private void EnableMovement()
    {
        movementEnabled = true;
        for (int i = 0; i < agentsLastPath.Count; i++)
        {
            agent.SetDestination(agentsLastPath[i]);
        }
        moving = true;
    }
    
    #endregion
    
}