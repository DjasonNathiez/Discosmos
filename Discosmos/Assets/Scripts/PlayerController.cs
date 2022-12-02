using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [SerializeField] private MovementType movementType = MovementType.moveToClickWithNavMesh;
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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = currentSpeed;
    }
    
    private void Update()
    {
        direction = transform.forward;
        currentSpeed = (speedCurve.Evaluate(force) - slowDownCurve.Evaluate(force)) + baseSpeed;
        
        
        MovementTypeSwitch();
        agent.speed = currentSpeed;
        Debug.DrawLine(transform.position, agent.destination, Color.yellow);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
    
    
    private enum MovementType
    {
        moveToClickWithNavMesh,
        KeepDirectionWithoutNavMesh,
        slide,
    }
    
    
    
    private void MovementTypeSwitch()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        switch (movementType)
        {
            case MovementType.moveToClickWithNavMesh:
                MoveToClickUsingTheNavMesh();
                break;
            case MovementType.KeepDirectionWithoutNavMesh:
                KeepDirectionNoNavMesh();
                break;
            case MovementType.slide:
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
}
