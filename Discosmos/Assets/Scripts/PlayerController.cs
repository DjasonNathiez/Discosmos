using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [Range(0,20)][SerializeField] public float speed;
    [Range(0,20)][SerializeField] public float controlSpeedChange;
    [SerializeField] private MovementType movementType = MovementType.moveToClickWithNavMesh;

    private Ray ray;
    private Ray rayCam;
    private RaycastHit hit;
    private Vector3 goal;
    
    private bool attacking =false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    
    private void Update()
    {
        //if the speed is > 10 set the enum MovementType to moveToClickbutKeepDirectionWithoutNavMesh else set it to moveToClickWithNavMesh
        if (speed > controlSpeedChange)
        {
            movementType = MovementType.moveToClickbutKeepDirectionWithoutNavMesh;
        }
        else
        {
            movementType = MovementType.moveToClickWithNavMesh;
        }
        
        
        MovementTypeSwitch();
        agent.speed = speed;
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
        moveToClickbutKeepDirectionWithoutNavMesh
    }
    
    
    
    private void MovementTypeSwitch()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        switch (movementType)
        {
            case MovementType.moveToClickWithNavMesh:
                MoveToClickUsingTheNavMesh();
                break;
            case MovementType.moveToClickbutKeepDirectionWithoutNavMesh:
                MoveToClickButKeepDirectionNoNavMesh();
                break;
        }
    }

    private void MoveToClickButKeepDirectionNoNavMesh()
    {
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.enabled = false;
                goal = hit.point;
                goal.y = transform.position.y;
            }
        }
        if (Vector3.Distance(transform.position, goal) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
            transform.LookAt(goal);
        }
        else
        {
            goal = transform.position + transform.forward * 10;
            transform.LookAt(goal);
        }
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
        //fix for when the agent run into a wall and wants to rotate
    }
}
