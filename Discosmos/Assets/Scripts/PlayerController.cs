using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPunObservable
{
    private NavMeshAgent agent;
    public bool clientPlayer;

    [Header("Stats and UI")] 
    
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Transform uiStatsTransform;
    [SerializeField] private float heightUI;

    public int healthMax;
    public int currentHealth;
    public int shield;
    
    [Header("Auto Attack")] 
    
    public int baseDamages;
    public int maxSpeedBonusDamages;
    public float range;
    public PlayerController cible;
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

    [SerializeField] public Camera _camera;

    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sparkles;

    [Header("UI")] 
    [SerializeField] private Image speedJauge;

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

            MovementTypeSwitch();
            speedJauge.fillAmount = force;
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
    }

    private void LateUpdate()
    {
        SetUI();
    }

    void SetUI()
    {
        uiStatsTransform.position = _camera.WorldToScreenPoint(transform.position + Vector3.up) + Vector3.up * heightUI;
        healthBar.fillAmount = currentHealth / (float) healthMax;
        healthText.text = currentHealth + " / " + healthMax;
    }


    private void MovementTypeSwitch()
    {
        if (_camera != null) ray = _camera.ScreenPointToRay(Input.mousePosition);
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
        }
    }

    public void OnEnterRamp(Rampe_LD rampeLd,bool forward,int startIndex)
    {
        onRamp = true;
        forwardOnRamp = forward;
        rampIndex = startIndex;
        rampProgress = 0;
        ramp = rampeLd;
        movementType = MovementType.Slide;
        agent.ResetPath();
        animator.Play("Slide");
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
        animator.Play("Roller");
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
            animator.Play("Idle");
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
            animator.Play("Idle");
        }
        
        
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                agent.SetDestination(hit.point);
                moving = true;
                if (force <= 0)
                {
                    animator.Play("Run");
                }
                else
                {
                    animator.Play("Roller");
                }
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            stream.ReceiveNext();
        }
    }
}
