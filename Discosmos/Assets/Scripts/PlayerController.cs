using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [SerializeField] private MovementType movementType = MovementType.MoveToClickWithNavMesh;
    [Range(0,1)][SerializeField] private float force; //from 0 to 1
    [Range(0, 20)] [SerializeField] public float baseSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve slowDownCurve;

    private Ray ray;
    private Ray rayCam;
    private RaycastHit hit;
    private Vector3 destination;
    private Vector3 direction;
    private float currentSpeed;
    
    private bool attacking =false;
    
    private bool speedPadTriggered = false;
    private Transform speedPad;
    [SerializeField] private float speedUp = 10;
    [SerializeField] private float slowDown = 2;
    [SerializeField] private float precisionAnglePad;
    [SerializeField] private bool speedPadPositive = false;
    [SerializeField] private float slowSpeedPadDuration = 0.5f;
    [SerializeField] private float fastSpeedPadDuration = 2;
    private float speedPadSpeed = 0;
    private double speedPadTimer = 0;
    private double time;
    [SerializeField] private float speedPadLerp = 1;
    
    [SerializeField] private Canvas canvas;
    
    
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = currentSpeed;
        //if photon is on, then we are in multiplayer
        SetTime();
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
        SetTime();
        
        direction = transform.forward;
        currentSpeed = (speedCurve.Evaluate(force) - slowDownCurve.Evaluate(force)) + baseSpeed + speedPadSpeed;
        ClampSpeed();
        agent.speed = currentSpeed;


        MovementTypeSwitch();
        Debug.DrawLine(transform.position, agent.destination, Color.yellow);

        SpeedPadFunction();

        //on the canvas, create a chart that shows the speed of the player 
        canvas.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(currentSpeed * 10, 10);
    }

    #region SpeedPad
    private void SpeedPadFunction()
    {
        if (speedPadTriggered)
        {
            SpeedPadEffect();
            speedPadTimer = 0;
        }

        if (!speedPadTriggered && speedPadSpeed != 0)
        {
            speedPadTimer += time;
            //if the speedPadSpeed is positive, then use the duration of the fast speed pad
            if (speedPadSpeed > 0)
            {
                if (speedPadTimer >= fastSpeedPadDuration)
                {
                    GoBackToNormalSpeed();
                }
            }
            //if the speedPadSpeed is negative, then use the duration of the slow speed pad
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
        speedPadSpeed = Mathf.Lerp(speedPadSpeed, 0, speedPadLerp);
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
        //debug Vector3.Angle(transform.forward, speedPad.forward) in the scene view
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.red);
        Debug.DrawLine(transform.position, transform.position + speedPad.forward * 10, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(precisionAnglePad, Vector3.up) * transform.forward * 10, Color.blue);
        
    }

    private void SlowDownPad()
    {
        speedPadSpeed = -slowDown;
        speedPadPositive = false;
        Debug.Log("slow down");
    }

    private void SpeedUpPad()
    {
        speedPadSpeed = speedUp;
        speedPadPositive = true;
        Debug.Log("speed up");
    }
    
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
    
    
    private enum MovementType
    {
        MoveToClickWithNavMesh,
        KeepDirectionWithoutNavMesh,
        Slide,
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

    public void Slide()
    {
        //ici fonc slide ^^
    }

    private void KeepDirectionNoNavMesh()
    {
        transform.LookAt(direction);
        transform.position += direction * (currentSpeed * Time.deltaTime);
            
    }

    
    private void MoveToClickUsingTheNavMesh()
    {
        agent.enabled = true;
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                agent.SetDestination(hit.point);
            }
        }
    }
    
    public float GetForce()
    {
        return force;
    }
    
    private void ClampSpeed()
    {
        if (currentSpeed > 20)
        {
            currentSpeed = 20;
        }
        else if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }
    
}
